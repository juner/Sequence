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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="jsonTypeInfo"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(Stream stream, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerReadOptions options, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            var reader = PipeReader.Create(stream);
            await using var enumerator = SequenceSerializer.DeserializeAsyncEnumerable(reader, jsonTypeInfo, options, cancellationToken).GetAsyncEnumerator(cancellationToken);
            var next = false;
            do
            {
                try {
                    next = await enumerator.MoveNextAsync();
                    if (!next)
                    {
                        await reader.CompleteAsync();
                        yield break;
                    }
                } catch (Exception e)
                {
                    await reader.CompleteAsync(e);
                    throw;
                }
                yield return enumerator.Current;

            } while (next);
            
        }
    }
}