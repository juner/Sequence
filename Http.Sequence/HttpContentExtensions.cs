using Juner.Sequence;
using System.IO.Pipelines;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Http.Sequence;

public static class HttpContentExtensions
{
    public static IAsyncEnumerable<T> ReadSequenceEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo<T> typeInfo,
        ISequenceSerializerReadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.IsEmpty) throw new ArgumentException(null, nameof(options));
        var stream = content.ReadAsStream(cancellationToken);
        var reader = PipeReader.Create(stream);

        return SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            typeInfo,
            options,
            cancellationToken);
    }

    public static IAsyncEnumerable<T> ReadJsonSequenceAsyncEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default)
    {

        var stream = content.ReadAsStream(cancellationToken);
        var reader = PipeReader.Create(stream);

        return SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            typeInfo,
            SequenceSerializerOptions.JsonSequence,
            cancellationToken);
    }

    public static IAsyncEnumerable<T> ReadJsonLinesAsyncEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default)
    {

        var stream = content.ReadAsStream(cancellationToken);
        var reader = PipeReader.Create(stream);

        return SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            typeInfo,
            SequenceSerializerOptions.JsonLines,
            cancellationToken);
    }
}