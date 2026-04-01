using Juner.Sequence;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Http.Sequence;

public static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Attach sequence content to the given <see cref="HttpRequestMessage"/>. The provided
    /// <paramref name="source"/> will be serialized using the supplied <paramref name="typeInfo"/>
    /// and framing options. The request's <see cref="System.Net.Http.HttpContent"/> will be set to
    /// a streaming content implementation.
    /// </summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    /// <param name="request">The request to modify.</param>
    /// <param name="source">The asynchronous sequence of values to be sent as the request body.</param>
    /// <param name="typeInfo">Json type information used to serialize elements.</param>
    /// <param name="options">Write options controlling framing and flush behaviour.</param>
    /// <param name="contentType">The media type to set on the request content.</param>
    /// <param name="cancellationToken">Cancellation token used by the streaming content.</param>
    /// <returns>The same <see cref="HttpRequestMessage"/> instance.</returns>
    public static HttpRequestMessage WithSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo<T> typeInfo,
        ISequenceSerializerWriteOptions options,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(typeInfo);
        ArgumentNullException.ThrowIfNull(options);
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
#else
        ArgumentNullException.ThrowIfNullOrEmpty(contentType);
#endif
        request.Content = new SequenceHttpContent<T>(
            source,
            typeInfo,
            contentType,
            options,
            cancellationToken);

        return request;
    }

    /// <summary>
    /// Attach JSON Sequence content (RFC 7464 / application/json-seq) to the request.
    /// </summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    /// <param name="request">The request to modify.</param>
    /// <param name="source">The asynchronous sequence of values to send.</param>
    /// <param name="typeInfo">Json type information for element serialization.</param>
    /// <param name="cancellationToken">Cancellation token used by the streaming content.</param>
    /// <returns>The same <see cref="HttpRequestMessage"/> instance.</returns>
    public static HttpRequestMessage WithJsonSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent<T>(
        request,
        source,
        typeInfo,
        SequenceSerializerOptions.JsonSequence,
        "application/json-seq",
        cancellationToken);

    /// <summary>
    /// Attach JSON Lines / JSONL content (newline-delimited JSON) to the request.
    /// </summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    /// <param name="request">The request to modify.</param>
    /// <param name="source">The asynchronous sequence of values to send.</param>
    /// <param name="typeInfo">Json type information for element serialization.</param>
    /// <param name="cancellationToken">Cancellation token used by the streaming content.</param>
    /// <returns>The same <see cref="HttpRequestMessage"/> instance.</returns>
    public static HttpRequestMessage WithJsonLinesContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent<T>(
        request,
        source,
        typeInfo,
        SequenceSerializerOptions.JsonLines,
        "application/jsonl",
        cancellationToken);

    /// <summary>
    /// Attach NDJSON (application/x-ndjson) content to the request.
    /// </summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    /// <param name="request">The request to modify.</param>
    /// <param name="source">The asynchronous sequence of values to send.</param>
    /// <param name="typeInfo">Json type information for element serialization.</param>
    /// <param name="cancellationToken">Cancellation token used by the streaming content.</param>
    /// <returns>The same <see cref="HttpRequestMessage"/> instance.</returns>
    public static HttpRequestMessage WithNdJsonContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent<T>(
        request,
        source,
        typeInfo,
        SequenceSerializerOptions.JsonLines,
        "application/x-ndjson",
        cancellationToken);
}