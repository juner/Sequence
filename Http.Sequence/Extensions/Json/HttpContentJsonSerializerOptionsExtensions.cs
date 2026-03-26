using Juner.Sequence;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Http.Sequence.Extensions.Json;

/// <summary>
/// 
/// </summary>
public static class HttpContentJsonSerializerOptionsExtensions
{
    const string RequiresUnreferencedCodeMessage = "Uses JsonSerializerOptions.Default which may require reflection.";
    const string RequiresDynamicCodeMessage = "May not be AOT compatible.";

    static string GenerateErrorMessage<T>()
     => $"JsonTypeInfo<{typeof(T).FullName}> not found. " +
        $"Ensure the type is registered in JsonSerializerOptions.TypeInfoResolver or source-generated context.";
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IAsyncEnumerable<T> ReadSequenceEnumerable<T>(
            this HttpContent content,
            JsonSerializerOptions jsonSerializerOptions,
            ISequenceSerializerReadOptions options,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);
        if (options.IsEmpty)
            throw new ArgumentException("Options must not be empty.", nameof(options));
        if (jsonSerializerOptions.GetTypeInfo(typeof(T)) is not JsonTypeInfo<T> jsonTypeInfo)
            throw new InvalidOperationException(GenerateErrorMessage<T>());
        return HttpContentExtensions.ReadSequenceEnumerable(
            content,
            jsonTypeInfo,
            options,
            cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IAsyncEnumerable<T> ReadJsonSequenceAsyncEnumerable<T>(
        this HttpContent content,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken = default)
        => ReadSequenceEnumerable<T>(
            content,
            jsonSerializerOptions,
            SequenceSerializerOptions.JsonSequence,
            cancellationToken
        );

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IAsyncEnumerable<T> ReadJsonLinesAsyncEnumerable<T>(
        this HttpContent content,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken = default)
        => ReadSequenceEnumerable<T>(
            content,
            jsonSerializerOptions,
            SequenceSerializerOptions.JsonLines,
            cancellationToken
        );
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static IAsyncEnumerable<T> ReadSequenceEnumerable<T>(
        this HttpContent content,
        ISequenceSerializerReadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        return ReadSequenceEnumerable<T>(
            content,
            JsonSerializerOptions.Default,
            options,
            cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static IAsyncEnumerable<T> ReadJsonSequenceAsyncEnumerable<T>(
        this HttpContent content,
        CancellationToken cancellationToken = default)
        => ReadSequenceEnumerable<T>(
            content,
            SequenceSerializerOptions.JsonSequence,
            cancellationToken
        );

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static IAsyncEnumerable<T> ReadJsonLinesAsyncEnumerable<T>(
        this HttpContent content,
        CancellationToken cancellationToken = default)
        => ReadSequenceEnumerable<T>(
            content,
            SequenceSerializerOptions.JsonLines,
            cancellationToken
        );
}
