using System.IO.Pipelines;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence;

public static partial class SequenceSerializer
{
    /// <summary>
    /// serialize sequence format
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
        var startIsEmpty = start.IsEmpty;
        var endIsEmpty = end.IsEmpty;
        if (options.FlushStrategy is FlushStrategy.PerRecord)
        {
            await foreach (var item in enumerable)
            {
                if (!startIsEmpty)
                    await writer.WriteAsync(start, cancellationToken);
                await JsonSerializer.SerializeAsync(writer, item, jsonTypeInfo, cancellationToken);
                if (!endIsEmpty)
                    await writer.WriteAsync(end, cancellationToken);
                await writer.FlushAsync(cancellationToken);
            }
            return;
        }
        await foreach (var item in enumerable)
        {
            if (!startIsEmpty)
                await writer.WriteAsync(start, cancellationToken);
            await JsonSerializer.SerializeAsync(writer, item, jsonTypeInfo, cancellationToken);
            if (!endIsEmpty)
                await writer.WriteAsync(end, cancellationToken);
        }
        await writer.FlushAsync(cancellationToken);
    }   
#else
    public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
        => SerializeAsync(writer.AsStream(), enumerable, jsonTypeInfo, options, cancellationToken);
#endif

    /// <summary>
    /// serialize sequence format
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
        var startIsEmpty = start.IsEmpty;
        var endIsEmpty = end.IsEmpty;

        if (options.FlushStrategy is FlushStrategy.PerRecord)
        {
            await foreach (var item in enumerable)
            {
                if (!startIsEmpty)
                    await stream.WriteAsync(start, cancellationToken);
                await JsonSerializer.SerializeAsync(stream, item, jsonTypeInfo, cancellationToken);
                if (!endIsEmpty)
                    await stream.WriteAsync(end, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            return;
        }
        await foreach (var item in enumerable)
        {
            if (!startIsEmpty)
                await stream.WriteAsync(start, cancellationToken);
            await JsonSerializer.SerializeAsync(stream, item, jsonTypeInfo, cancellationToken);
            if (!endIsEmpty)
                await stream.WriteAsync(end, cancellationToken);
        }
        await stream.FlushAsync(cancellationToken);
    }
}
