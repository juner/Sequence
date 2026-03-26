using Juner.Sequence;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Http.Sequence;

public static class HttpRequestMessageExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="typeInfo"></param>
    /// <param name="options"></param>
    /// <param name="contentType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="typeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="typeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="typeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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