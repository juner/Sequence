using System.IO.Pipelines;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence.Extensions;

/// <summary>
/// <see cref="SequenceSerializer"/> use <see cref="JsonSerializerOptions"/> extensions
/// </summary>
public static class SequenceSerializerJsonSerializerOptionsExtensions
{
    static string GenerateErrorMessage<T>()
     => $"JsonTypeInfo<{typeof(T).FullName}> not found. " +
        $"Ensure the type is registered in JsonSerializerOptions.TypeInfoResolver.";
    extension(SequenceSerializer)
    {
        /// <summary>
        /// Serialize an asynchronous sequence using a <see cref="JsonSerializerOptions"/> instance to
        /// obtain the required <see cref="JsonTypeInfo{T}"/>. Throws when the type is not registered in the
        /// provided options.
        /// </summary>
        /// <typeparam name="T">The element type to serialize.</typeparam>
        /// <param name="writer">Destination <see cref="PipeWriter"/>.</param>
        /// <param name="enumerable">The asynchronous sequence to serialize.</param>
        /// <param name="jsonSerializerOptions">JsonSerializerOptions that contain type information.</param>
        /// <param name="options">Write options controlling framing and flush behaviour.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous serialize operation.</returns>
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonSerializerOptions jsonSerializerOptions, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
        {
#if !NET8_0_OR_GREATER
            jsonSerializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
#endif
            return jsonSerializerOptions.GetTypeInfo(typeof(T)) switch
            {
                JsonTypeInfo<T> jsonTypeInfo => SequenceSerializer.SerializeAsync(writer, enumerable, jsonTypeInfo, options, cancellationToken),
                _ => throw new InvalidOperationException(GenerateErrorMessage<T>()),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, JsonSerializerOptions jsonSerializerOptions, ISequenceSerializerReadOptions options, CancellationToken cancellationToken = default)
        {
#if !NET8_0_OR_GREATER
            jsonSerializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
#endif
            return jsonSerializerOptions.GetTypeInfo(typeof(T)) switch
            {
                JsonTypeInfo<T> jsonTypeInfo => SequenceSerializer.DeserializeAsyncEnumerable(reader, jsonTypeInfo, options, cancellationToken),
                _ => throw new InvalidOperationException(GenerateErrorMessage<T>()),
            };
        }
    }
}