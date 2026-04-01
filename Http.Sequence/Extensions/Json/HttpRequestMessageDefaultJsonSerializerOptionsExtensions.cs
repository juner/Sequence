using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Juner.Sequence;

namespace Juner.Http.Sequence.Extensions.Json;

/// <summary>
/// 
/// </summary>
public static class HttpRequestMessageDefaultJsonSerializerOptionsExtensions
{
    const string RequiresUnreferencedCodeMessage = "Uses JsonSerializerOptions.Default which may require reflection.";
    const string RequiresDynamicCodeMessage = "May not be AOT compatible.";

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <param name="contentType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static HttpRequestMessage WithSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        ISequenceSerializerWriteOptions options,
        string contentType,
        CancellationToken cancellationToken = default)
        => HttpRequestMessageJsonSerializerOptionsExtensions.WithSequenceContent(
            request,
            source,
            JsonSerializerOptions.Default,
            options,
            contentType,
            cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static HttpRequestMessage WithJsonSequenceContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent(
        request,
        source,
        SequenceSerializerOptions.JsonSequence,
        "application/json-seq",
        cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static HttpRequestMessage WithJsonLinesContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent(
        request,
        source,
        SequenceSerializerOptions.JsonLines,
        "application/jsonl",
        cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static HttpRequestMessage WithNdJsonContent<T>(
        this HttpRequestMessage request,
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default
    ) => WithSequenceContent(
        request,
        source,
        SequenceSerializerOptions.JsonLines,
        "application/x-ndjson",
        cancellationToken);
}