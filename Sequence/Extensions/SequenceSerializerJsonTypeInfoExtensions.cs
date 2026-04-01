using System.IO.Pipelines;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence.Extensions;

/// <summary>
/// <see cref="SequenceSerializer"/> use no generic <see cref="JsonTypeInfo"/> extensions
/// </summary>
public static class SequenceSerializerJsonTypeInfoExtensions
{
    extension(SequenceSerializer)
    {
        /// <summary>
        /// Serialize the provided asynchronous sequence using a non-generic <see cref="JsonTypeInfo"/>.
        /// This method attempts to cast the supplied <paramref name="jsonTypeInfo"/> to the expected generic
        /// type and delegates to <see cref="SequenceSerializer.SerializeAsync{T}(System.IO.Pipelines.PipeWriter, IAsyncEnumerable{T}, System.Text.Json.Serialization.Metadata.JsonTypeInfo{T}, ISequenceSerializerWriteOptions, CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence to serialize.</typeparam>
        /// <param name="writer">The destination <see cref="PipeWriter"/>.</param>
        /// <param name="enumerable">The asynchronous sequence to serialize.</param>
        /// <param name="jsonTypeInfo">A non-generic <see cref="JsonTypeInfo"/> instance for the element type.</param>
        /// <param name="options">Write options controlling delimiters and flushing.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that represents the asynchronous serialize operation.</returns>
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonTypeInfo jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
         => jsonTypeInfo switch
         {
             JsonTypeInfo<T> jsonTypeInfo2 => SequenceSerializer.SerializeAsync(writer, enumerable, jsonTypeInfo2, options, cancellationToken),
             _ => throw new InvalidOperationException(),
         };

        /// <summary>
        /// Deserialize an asynchronous sequence from the given <see cref="PipeReader"/> using a non-generic <see cref="JsonTypeInfo"/>.
        /// Attempts to cast the provided <paramref name="jsonTypeInfo"/> to the expected generic type and delegates to
        /// <see cref="SequenceSerializer.DeserializeAsyncEnumerable{T}(System.IO.Pipelines.PipeReader, System.Text.Json.Serialization.Metadata.JsonTypeInfo{T}, ISequenceSerializerReadOptions, CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The element type to deserialize.</typeparam>
        /// <param name="reader">The source <see cref="PipeReader"/>.</param>
        /// <param name="jsonTypeInfo">A non-generic <see cref="JsonTypeInfo"/> for the element type.</param>
        /// <param name="options">Read options controlling delimiters and behavior on incomplete frames.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An asynchronous sequence of deserialized elements.</returns>
        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, JsonTypeInfo jsonTypeInfo, ISequenceSerializerReadOptions options, CancellationToken cancellationToken = default)
         => jsonTypeInfo switch
         {
             JsonTypeInfo<T> jsonTypeInfo2 => SequenceSerializer.DeserializeAsyncEnumerable(reader, jsonTypeInfo2, options, cancellationToken),
             _ => throw new InvalidOperationException(),
         };
    }
}