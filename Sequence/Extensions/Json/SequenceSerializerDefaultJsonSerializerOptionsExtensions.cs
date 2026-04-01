using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text.Json;

namespace Juner.Sequence.Extensions.Json;

/// <summary>
/// <see cref="SequenceSerializer"/> use <see cref="JsonSerializerOptions"/> extensions
/// </summary>
public static class SequenceSerializerDefaultJsonSerializerOptionsExtensions
{
    const string RequiresUnreferencedCodeMessage = "Uses JsonSerializerOptions.Default which may require reflection.";
    const string RequiresDynamicCodeMessage = "May not be AOT compatible.";
    extension(SequenceSerializer)
    {

        /// <summary>
        /// Serialize an asynchronous sequence using <see cref="JsonSerializerOptions.Default"/>.
        /// This method may require reflection-based serialization support and therefore is marked
        /// with trimming and dynamic code warnings.
        /// </summary>
        /// <typeparam name="T">The element type to serialize.</typeparam>
        /// <param name="writer">Destination <see cref="System.IO.Pipelines.PipeWriter"/>.</param>
        /// <param name="enumerable">The asynchronous sequence to serialize.</param>
        /// <param name="options">Write options controlling framing and flush behaviour.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous serialize operation.</returns>
        [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
        [RequiresDynamicCode(RequiresDynamicCodeMessage)]
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
            => SequenceSerializerJsonSerializerOptionsExtensions.SerializeAsync(writer, enumerable, JsonSerializerOptions.Default, options, cancellationToken);

        /// <summary>
        /// Deserialize an asynchronous sequence using <see cref="JsonSerializerOptions.Default"/>.
        /// This may require reflection-based support and is marked accordingly.
        /// </summary>
        /// <typeparam name="T">The element type to deserialize.</typeparam>
        /// <param name="reader">The source <see cref="System.IO.Pipelines.PipeReader"/>.</param>
        /// <param name="options">Read options controlling framing and incomplete frame handling.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An asynchronous sequence of deserialized elements.</returns>
        [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
        [RequiresDynamicCode(RequiresDynamicCodeMessage)]
        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, ISequenceSerializerReadOptions options, CancellationToken cancellationToken = default)
            => SequenceSerializerJsonSerializerOptionsExtensions.DeserializeAsyncEnumerable<T>(reader, JsonSerializerOptions.Default, options, cancellationToken);
    }
}