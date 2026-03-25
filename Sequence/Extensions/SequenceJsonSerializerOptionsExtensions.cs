using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence.Extensions;

/// <summary>
/// 
/// </summary>
public static class SequenceJsonSerializerOptionsExtensions
{
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
                JsonTypeInfo<T> jsonTypeInfo => SequenceSerializer.SerializeAsync<T>(writer, enumerable, jsonTypeInfo, options, cancellationToken),
                _ => throw new InvalidOperationException(
                    $"JsonTypeInfo<{typeof(T).FullName}> not found. " +
                    $"Ensure the type is registered in JsonSerializerOptions.TypeInfoResolver."),
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
                JsonTypeInfo<T> jsonTypeInfo => SequenceSerializer.DeserializeAsyncEnumerable<T>(reader, jsonTypeInfo, options, cancellationToken),
                _ => throw new InvalidOperationException(
                    $"JsonTypeInfo<{typeof(T).FullName}> not found. " +
                    $"Ensure the type is registered in JsonSerializerOptions.TypeInfoResolver."),
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
        [RequiresUnreferencedCode("Uses JsonSerializerOptions which may require reflection.")]
        [RequiresDynamicCode("May not be AOT compatible.")]
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
            => SerializeAsync<T>(writer, enumerable, JsonSerializerOptions.Default, options, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresUnreferencedCode("Uses JsonSerializerOptions which may require reflection.")]
        [RequiresDynamicCode("May not be AOT compatible.")]
        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, ISequenceSerializerReadOptions options, CancellationToken cancellationToken = default)
            => DeserializeAsyncEnumerable<T>(reader, JsonSerializerOptions.Default, options, cancellationToken);
    }
}