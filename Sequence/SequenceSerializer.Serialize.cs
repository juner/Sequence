using System.IO.Pipelines;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence;

public static partial class SequenceSerializer
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer"></param>
    /// <param name="enumerable"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
#if NET9_0_OR_GREATER
    public static async Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
    {

        if (enumerable is null) return;
        var start = options.Start;
        var end = options.End;
        await foreach (var item in enumerable)
        {
            if (start is not { IsEmpty: true })
                await writer.WriteAsync(start, cancellationToken);
            await JsonSerializer.SerializeAsync(writer, item, jsonTypeInfo, cancellationToken);
            if (end is not { IsEmpty: true })
                await writer.WriteAsync(end, cancellationToken);
        }
    }   
#else
    public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
        => SerializeAsync(writer.AsStream(), enumerable, jsonTypeInfo, options, cancellationToken);
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="enumerable"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task SerializeAsync<T>(Stream stream, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
    {
        if (enumerable is null) return;
        var start = options.Start;
        var end = options.End;
        await foreach (var item in enumerable)
        {
            if (start is not { IsEmpty: true })
                await stream.WriteAsync(start, cancellationToken);
            await JsonSerializer.SerializeAsync(stream, item, jsonTypeInfo, cancellationToken);
            if (end is not { IsEmpty: true })
                await stream.WriteAsync(end, cancellationToken);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer"></param>
    /// <param name="enumerable"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="options"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, Encoding? encoding, CancellationToken cancellationToken = default)
    {
        encoding ??= Encoding.UTF8;
        if (Encoding.UTF8 == encoding)
        {
            return SerializeAsync(writer, enumerable, jsonTypeInfo, options, cancellationToken);
        }
        return WrappedSerializeAsync(Encoding.CreateTranscodingStream(writer.AsStream(), encoding, Encoding.UTF8, leaveOpen: true), enumerable, jsonTypeInfo, options, cancellationToken);
        static async Task WrappedSerializeAsync(Stream stream, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
        {
            var writer = PipeWriter.Create(stream);
            ExceptionDispatchInfo? exceptionDispatchInfo = null;
            try
            {
                await SerializeAsync(writer, enumerable, jsonTypeInfo, options, cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                try
                {
                    await stream.DisposeAsync();
                }
                catch when (exceptionDispatchInfo != null)
                {
                }
                exceptionDispatchInfo?.Throw();
            }
        }
    }
}