using Juner.Sequence;
using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Channels;

namespace Juner.AspNetCore.Sequence.Internals;

internal class InternalFormatReader
{
    #region ReadResult
    public static object ReadResult<T>(
        EnumerableType enumerableType,
        PipeReader reader,
        JsonTypeInfo jsonTypeInfo,
        ISequenceSerializerReadOptions options,
        CancellationToken cancellationToken)
    {
        var asyncEnumerable = SequenceSerializer.DeserializeAsyncEnumerable(reader, (JsonTypeInfo<T>)jsonTypeInfo, options, cancellationToken);
        return enumerableType switch
        {
            EnumerableType.AsyncEnumerable => asyncEnumerable,
            EnumerableType.Sequence => new Http.Sequence<T>(asyncEnumerable),
            EnumerableType.Enumerable => GetEnumerableAsync(asyncEnumerable, cancellationToken),
            EnumerableType.Array => GetArrayAsync(asyncEnumerable, cancellationToken),
            EnumerableType.List => GetListAsync(asyncEnumerable, cancellationToken),
            EnumerableType.ChannelReader => GetChannelReader(asyncEnumerable, cancellationToken),
            _ => throw new InvalidOperationException($"type:{enumerableType} is not support"),
        };
    }

    public static object ReadResult(
        Type elementType,
        EnumerableType enumerableType,
        PipeReader reader,
        JsonTypeInfo jsonTypeInfo,
        ISequenceSerializerReadOptions options,
        CancellationToken cancellationToken
    )
    {
        var del = cache.GetOrAdd(elementType, CreateDelegate);

        var func =
            (Func<
                EnumerableType,
                PipeReader,
                JsonTypeInfo,
                ISequenceSerializerReadOptions,
                CancellationToken,
                object>)del;

        return func(
            enumerableType,
            reader,
            jsonTypeInfo,
            options,
            cancellationToken);
    }

    static readonly ConcurrentDictionary<Type, Delegate> cache = new();

    static Delegate CreateDelegate(Type elementType)
    {
        var method =
            typeof(InternalFormatReader)
            .GetMethods()
            .First(static v => v is
            {
                Name: nameof(ReadResult),
                IsGenericMethod: true
            } && v.GetParameters() is { Length: 5 })
            .MakeGenericMethod(elementType);

        return method.CreateDelegate(
            typeof(Func<
                EnumerableType,
                PipeReader,
                JsonTypeInfo,
                ISequenceSerializerReadOptions,
                CancellationToken,
                object>));
    }
    #endregion

    public static async Task<IEnumerable<T>> GetEnumerableAsync<T>(
        IAsyncEnumerable<T> asyncEnumerable,
        CancellationToken cancellationToken)
    {
#if NET10_0_OR_GREATER
        return await asyncEnumerable.ToListAsync(cancellationToken);
#else
        List<T>? list = null;
        await foreach (var item in asyncEnumerable.WithCancellation(cancellationToken))
            (list ??= []).Add(item);
        return list ?? [];
#endif
    }

    public static async Task<List<T>> GetListAsync<T>(
        IAsyncEnumerable<T> asyncEnumerable,
        CancellationToken cancellationToken)
    {
#if NET10_0_OR_GREATER
        return await asyncEnumerable.ToListAsync(cancellationToken);
#else
        List<T>? list = null;
        await foreach (var item in asyncEnumerable.WithCancellation(cancellationToken))
            (list ??= []).Add(item);
        return list ?? [];
#endif
    }

    public static async Task<T[]> GetArrayAsync<T>(
        IAsyncEnumerable<T> asyncEnumerable,
        CancellationToken cancellationToken)
    {
#if NET10_0_OR_GREATER
        return await asyncEnumerable.ToArrayAsync(cancellationToken);
#else
        List<T>? list = null;
        await foreach (var item in asyncEnumerable.WithCancellation(cancellationToken))
            (list ??= []).Add(item);
        return list?.ToArray() ?? Array.Empty<T>();
#endif
    }

    public static async Task<ChannelReader<T>> GetChannelReader<T>(
        IAsyncEnumerable<T> asyncEnumerable,
        CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<T>();

        _ = Task.Run(async () =>
        {
            Exception? error = null;
            try
            {
                await foreach (var item in asyncEnumerable.WithCancellation(cancellationToken))
                    await channel.Writer.WriteAsync(item);
            }
            catch (Exception error2)
            {
                error = error2;
            }
            finally
            {
                channel.Writer.Complete(error);
            }
        }, cancellationToken);

        return channel.Reader;
    }
}