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
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, JsonTypeInfo jsonTypeInfo, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
         => jsonTypeInfo switch
         {
             JsonTypeInfo<T> jsonTypeInfo2 => SequenceSerializer.SerializeAsync(writer, enumerable, jsonTypeInfo2, options, cancellationToken),
             _ => throw new InvalidOperationException(),
         };

        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, JsonTypeInfo jsonTypeInfo, ISequenceSerializerReadOptions options, CancellationToken cancellationToken = default)
         => jsonTypeInfo switch
         {
             JsonTypeInfo<T> jsonTypeInfo2 => SequenceSerializer.DeserializeAsyncEnumerable(reader, jsonTypeInfo2, options, cancellationToken),
             _ => throw new InvalidOperationException(),
         };
    }
}