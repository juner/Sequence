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