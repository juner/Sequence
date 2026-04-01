using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence.Extensions;

public static class StreamExtensions
{
    extension(SequenceSerializer)
    {
        /// <summary>
        /// Serialize an asynchronous sequence to the provided <see cref="Stream"/>.
        /// This is a stream-based wrapper that uses <see cref="JsonSerializer"/> for element serialization
        /// and respects framing and flush behaviour from <paramref name="options"/>.
        /// </summary>
        /// <typeparam name="T">The element type to serialize.</typeparam>
        /// <param name="stream">The destination stream.</param>
        /// <param name="enumerable">The asynchronous sequence to serialize.</param>
        /// <param name="jsonTypeInfo">Json type information for serialization.</param>
        /// <param name="options">Write options for framing and flushing.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous serialize operation.</returns>
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

        /// <summary>
        /// Deserialize an asynchronous sequence from the provided <see cref="Stream"/>.
        /// This creates a <see cref="PipeReader"/> over the stream and delegates to the
        /// <see cref="SequenceSerializer.DeserializeAsyncEnumerable{T}(System.IO.Pipelines.PipeReader, System.Text.Json.Serialization.Metadata.JsonTypeInfo{T}, ISequenceSerializerReadOptions, CancellationToken)"/>
        /// implementation.
        /// </summary>
        /// <typeparam name="T">The element type to deserialize.</typeparam>
        /// <param name="reader">The source stream to read from.</param>
        /// <param name="jsonTypeInfo">Json type information for deserialization.</param>
        /// <param name="options">Read options controlling delimiters and incomplete frame handling.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An asynchronous sequence of deserialized elements.</returns>
        public static async IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(Stream stream, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerReadOptions options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var reader = PipeReader.Create(stream);
            await using var enumerator = SequenceSerializer.DeserializeAsyncEnumerable(reader, jsonTypeInfo, options, cancellationToken).GetAsyncEnumerator(cancellationToken);
            var next = false;
            do
            {
                try
                {
                    next = await enumerator.MoveNextAsync();
                    if (!next)
                    {
                        await reader.CompleteAsync();
                        yield break;
                    }
                }
                catch (Exception e)
                {
                    await reader.CompleteAsync(e);
                    throw;
                }
                yield return enumerator.Current;

            } while (next);

        }
    }
}