using Juner.Sequence;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Http.Sequence.Extensions;

/// <summary>
/// 
/// </summary>
public static class HttpContentJsonTypeInfoExtensions
{
    static string GetErrorMessage<T>(JsonTypeInfo jsonTypeInfo) => $"JsonTypeInfo<{typeof(T).FullName}> expected but got {jsonTypeInfo.GetType()}";
    
    public static IAsyncEnumerable<T> ReadSequenceEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo jsonTypeInfo,
        ISequenceSerializerReadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);
        if (options.IsInvalid)
            throw new ArgumentException("Options must not be empty.", nameof(options));
        if (jsonTypeInfo is not JsonTypeInfo<T> jsonTypeInfo2)
            throw new InvalidOperationException(GetErrorMessage<T>(jsonTypeInfo));
        return HttpContentExtensions.ReadSequenceEnumerable(
            content,
            jsonTypeInfo2,
            options,
            cancellationToken);
    }

    public static IAsyncEnumerable<T> ReadJsonSequenceAsyncEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo jsonTypeInfo,
        CancellationToken cancellationToken = default
    ) => ReadSequenceEnumerable<T>(
        content,
        jsonTypeInfo,
        SequenceSerializerOptions.JsonSequence,
        cancellationToken
    );

    public static IAsyncEnumerable<T> ReadJsonLinesAsyncEnumerable<T>(
        this HttpContent content,
        JsonTypeInfo jsonTypeInfo,
        CancellationToken cancellationToken = default
    ) => ReadSequenceEnumerable<T>(
        content,
        jsonTypeInfo,
        SequenceSerializerOptions.JsonLines,
        cancellationToken
    );
}
