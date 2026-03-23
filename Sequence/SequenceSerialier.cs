using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Channels;

namespace Juner.Sequence;

public static class SequenceSerialier
{
    public static Task SerializeAsync<T>(IAsyncEnumerable<T> enumerable, JsonTypeInfo<T> jsonTypeInfo, SequenceSerializerOptions options, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public static IAsyncEnumerable<T> DeserializeAsync<T>(PipeReader stream, JsonTypeInfo<T> jsonTypeInfo, SequenceSerializerOptions options, CancellationToken cancellationToken)
        => throw new NotImplementedException();
    public static IAsyncEnumerable<T> DeserializeAsync<T>(Stream stream, JsonTypeInfo<T> jsonTypeInfo, SequenceSerializerOptions options, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}

public static class SequenceSerializerExtensions
{
    extension(SequenceSerialier)
    {
        public static Task SerializeAsync<T>(ChannelReader<T> reader, JsonTypeInfo<T> jsonTypeInfo, SequenceSerializerOptions options, CancellationToken cancellationToken)
                => SequenceSerialier.SerializeAsync(ToAsyncEnumerable(reader, cancellationToken), jsonTypeInfo, options, cancellationToken);

    }
    static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(ChannelReader<T> reader, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (await reader.WaitToReadAsync(cancellationToken))
        {
            if (reader.TryRead(out var item)) yield return item;
        }
    }
}
