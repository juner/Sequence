using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence.Extensions.Json;

/// <summary>
/// <see cref="SequenceSerializer"/> use <see cref="JsonSerializerOptions"/> extensions
/// </summary>
public static class SequenceJsonSerializerOptionsExtensions
{
    const string RequiresUnreferencedCodeMessage = "Uses JsonSerializerOptions.Default which may require reflection.";
    const string RequiresDynamicCodeMessage = "May not be AOT compatible.";
    static string GenerateErrorMessage<T>()
     => $"JsonTypeInfo<{typeof(T).FullName}> not found. " +
        $"Ensure the type is registered in JsonSerializerOptions.TypeInfoResolver.";
    extension(SequenceSerializer)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="enumerable"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonSerializerOptions jsonSerializerOptions, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
            => jsonSerializerOptions.GetTypeInfo(typeof(T)) switch
            {
                JsonTypeInfo<T> jsonTypeInfo => SequenceSerializer.SerializeAsync(writer, enumerable, jsonTypeInfo, options, cancellationToken),
                _ => throw new InvalidOperationException(GenerateErrorMessage<T>()),
            };

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
            => jsonSerializerOptions.GetTypeInfo(typeof(T)) switch
            {
                JsonTypeInfo<T> jsonTypeInfo => SequenceSerializer.DeserializeAsyncEnumerable(reader, jsonTypeInfo, options, cancellationToken),
                _ => throw new InvalidOperationException(GenerateErrorMessage<T>()),
            };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="enumerable"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
        [RequiresDynamicCode(RequiresDynamicCodeMessage)]
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
            => SerializeAsync(writer, enumerable, JsonSerializerOptions.Default, options, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
        [RequiresDynamicCode(RequiresDynamicCodeMessage)]
        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, ISequenceSerializerReadOptions options, CancellationToken cancellationToken = default)
            => DeserializeAsyncEnumerable<T>(reader, JsonSerializerOptions.Default, options, cancellationToken);
    }
}