using System.IO.Pipelines;
using System.Text.Json.Serialization.Metadata;

using Juner.Sequence;

namespace Juner.Http.Sequence;

public static class HttpContentExtensions
{
    /// <summary>
    /// Read the content as a streaming sequence and deserialize elements using the provided <see cref="JsonTypeInfo{T}"/>.
    /// </summary>
    /// <typeparam name="T">The element type to deserialize.</typeparam>
    /// <param name="content">The HTTP content to read from.</param>
    /// <param name="typeInfo">Json type information for deserialization.</param>
    /// <param name="options">Read options controlling delimiters and incomplete frame handling.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An asynchronous sequence of deserialized elements.</returns>
    public static IAsyncEnumerable<T> ReadSequenceEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo<T> typeInfo,
        ISequenceSerializerReadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.IsInvalid) throw new ArgumentException(null, nameof(options));
        var stream = content.ReadAsStream(cancellationToken);
        var reader = PipeReader.Create(stream);

        return SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            typeInfo,
            options,
            cancellationToken);
    }

    /// <summary>
    /// Read the content as an "application/json-seq" stream and deserialize elements.
    /// </summary>
    public static IAsyncEnumerable<T> ReadJsonSequenceAsyncEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default)
        => ReadSequenceEnumerable<T>(
            content,
            typeInfo,
            SequenceSerializerOptions.JsonSequence,
            cancellationToken
        );

    /// <summary>
    /// Read the content as newline-delimited JSON (JSON Lines / NDJSON) and deserialize elements.
    /// </summary>
    public static IAsyncEnumerable<T> ReadJsonLinesAsyncEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default)
        => ReadSequenceEnumerable<T>(
            content,
            typeInfo,
            SequenceSerializerOptions.JsonLines,
            cancellationToken
        );
}