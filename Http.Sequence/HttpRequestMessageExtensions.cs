using Juner.Sequence;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Http.Sequence;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessage WithSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo<T> typeInfo,
        ISequenceSerializerWriteOptions options,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(contentType);
        request.Content = new SequenceHttpContent<T>(
            source,
            typeInfo,
            contentType,
            options,
            cancellationToken);

        return request;
    }

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