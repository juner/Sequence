using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Juner.Sequence;

namespace Juner.Http.Sequence.Extensions.Json;

/// <summary>
/// 
/// </summary>
public static class HttpRequestMessageJsonSerializerOptionsExtensions
{
    static string GenerateErrorMessage<T>()
     => $"JsonTypeInfo<{typeof(T).FullName}> not found. " +
        $"Ensure the type is registered in JsonSerializerOptions.TypeInfoResolver or source-generated context.";

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="options"></param>
    /// <param name="contentType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static HttpRequestMessage WithSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonSerializerOptions jsonSerializerOptions,
        ISequenceSerializerWriteOptions options,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
#else
        ArgumentNullException.ThrowIfNullOrEmpty(contentType);
#endif
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);
        if (options.IsInvalid)
            throw new ArgumentException("Options must not be empty.", nameof(options));
#if !NET8_0_OR_GREATER
        jsonSerializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
#endif
        if (jsonSerializerOptions.GetTypeInfo(typeof(T)) is not JsonTypeInfo<T> jsonTypeInfo)
            throw new InvalidOperationException(GenerateErrorMessage<T>());
        return HttpRequestMessageExtensions.WithSequenceContent(
            request,
            source,
            jsonTypeInfo,
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
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static HttpRequestMessage WithJsonSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent(request,
        source,
        jsonSerializerOptions,
        SequenceSerializerOptions.JsonSequence,
        "application/json-seq",
        cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static HttpRequestMessage WithJsonLinesContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent(request,
        source,
        jsonSerializerOptions,
        SequenceSerializerOptions.JsonLines,
        "application/jsonl",
        cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static HttpRequestMessage WithNdJsonContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent(request,
        source,
        jsonSerializerOptions,
        SequenceSerializerOptions.JsonLines,
        "application/x-ndjson",
        cancellationToken);
}