using System.IO.Pipelines;
using System.Text.Json.Serialization.Metadata;

#if NET9_0_OR_GREATER
using System.Text.Json;
#endif
namespace Juner.Sequence;

public static partial class SequenceSerializer
{
    /// <summary>
    /// Serialize an asynchronous sequence of objects into the provided <see cref="PipeWriter"/>.
    /// Elements are serialized using the supplied <see cref="JsonTypeInfo{T}"/> and the behaviour
    /// for framing and flushing is controlled by <paramref name="options"/>.
    /// </summary>
    /// <typeparam name="T">The element type to serialize.</typeparam>
    /// <param name="writer">The destination <see cref="PipeWriter"/>.</param>
    /// <param name="enumerable">The asynchronous sequence to serialize.</param>
    /// <param name="jsonTypeInfo">The JsonTypeInfo used for serializing elements.</param>
    /// <param name="options">Write options that control delimiters and flush strategy.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for async operations.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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