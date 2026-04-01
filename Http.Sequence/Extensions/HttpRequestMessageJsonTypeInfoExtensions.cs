using System.Text.Json.Serialization.Metadata;

using Juner.Sequence;

namespace Juner.Http.Sequence.Extensions;

/// <summary>
/// 
/// </summary>
public static class HttpRequestMessageJsonTypeInfoExtensions
{
    static string GetErrorMessage<T>(JsonTypeInfo jsonTypeInfo) => $"JsonTypeInfo<{typeof(T).FullName}> expected but got {jsonTypeInfo.GetType()}";

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="options"></param>
    /// <param name="contentType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static HttpRequestMessage WithSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo jsonTypeInfo,
        ISequenceSerializerWriteOptions options,
        string contentType,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
#else
        ArgumentNullException.ThrowIfNullOrEmpty(contentType);
#endif
        if (options.IsInvalid)
            throw new ArgumentException("Options must not be empty.", nameof(options));
        if (jsonTypeInfo is not JsonTypeInfo<T> jsonTypeInfo2)
            throw new InvalidOperationException(GetErrorMessage<T>(jsonTypeInfo));
        return HttpRequestMessageExtensions.WithSequenceContent<T>(
            request,
            source,
            jsonTypeInfo2,
            options,
            contentType,
            cancellationToken
        );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static HttpRequestMessage WithJsonSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo jsonTypeInfo,
        CancellationToken cancellationToken
    ) => WithSequenceContent(
        request,
        source,
        jsonTypeInfo,
        SequenceSerializerOptions.JsonSequence,
        "application/json-seq",
        cancellationToken
    );

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static HttpRequestMessage WithJsonLinesContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo jsonTypeInfo,
        CancellationToken cancellationToken
    ) => WithSequenceContent(
        request,
        source,
        jsonTypeInfo,
        SequenceSerializerOptions.JsonLines,
        "application/jsonl",
        cancellationToken
    );

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static HttpRequestMessage WithNdJsonContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonTypeInfo jsonTypeInfo,
        CancellationToken cancellationToken
    ) => WithSequenceContent(
        request,
        source,
        jsonTypeInfo,
        SequenceSerializerOptions.JsonLines,
        "application/x-ndjson",
        cancellationToken
    );
}