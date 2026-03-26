using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence.Extensions;

/// <summary>
/// <see cref="SequenceSerializer"/>  Change <see cref="Encoding"/> Extensions
/// </summary>
public static class SequenceSerializerEncodeExntensions
{
    extension(SequenceSerializer)
    {
        /// <summary>
        /// serialize sequence format
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
            if (Encoding.UTF8.CodePage == encoding.CodePage)
            {
                return SequenceSerializer.SerializeAsync(writer, enumerable, jsonTypeInfo, options, cancellationToken);
            }
            return WrappedSerializeAsync(Encoding.CreateTranscodingStream(writer.AsStream(), encoding, Encoding.UTF8, leaveOpen: true), enumerable, jsonTypeInfo, options, cancellationToken);
            static async Task WrappedSerializeAsync(Stream stream, IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
            {
                await using (stream)
                    await SequenceSerializer.SerializeAsync(stream, enumerable, jsonTypeInfo, options, cancellationToken);
            }
        }

        /// <summary>
        /// deserialize sequence format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="jsonTypeInfo"></param>
        /// <param name="options"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerReadOptions options, Encoding? encoding, CancellationToken cancellationToken = default)
        {
            encoding ??= Encoding.UTF8;
            if (Encoding.UTF8.CodePage == encoding.CodePage)
            {
                return SequenceSerializer.DeserializeAsyncEnumerable(reader, jsonTypeInfo, options, cancellationToken);
            }
            return WrappedDeserializeAsyncEnumerable(Encoding.CreateTranscodingStream(reader.AsStream(), encoding, Encoding.UTF8, leaveOpen: true), jsonTypeInfo, options, cancellationToken);
            static async IAsyncEnumerable<T> WrappedDeserializeAsyncEnumerable(Stream stream, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerReadOptions options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                await using (stream)
                    await foreach (var item in SequenceSerializer
                        .DeserializeAsyncEnumerable(stream, jsonTypeInfo, options, cancellationToken)
                        .WithCancellation(cancellationToken))
                        yield return item;

            }
        }
    }
}
