using System.IO.Pipelines;
using System.Text.Json.Serialization.Metadata;

#if NET9_0_OR_GREATER
using System.Text.Json;
#endif
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
    public static async Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
    {
        await using var stream = writer.AsStream(leaveOpen: true);
        await Extensions.StreamExtensions.SerializeAsync(writer.AsStream(), enumerable, jsonTypeInfo, options, cancellationToken);
    }
#endif
}