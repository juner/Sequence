namespace Juner.AspNetCore.Sequence.Http;

public interface IContent
{
    /// <summary>
    /// The media type string for this content (for example "application/json").
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// True when the content is sent as a streaming sequence (for example NDJSON or JSON Sequence).
    /// False when the content is sent as a single non-streaming payload.
    /// </summary>
    bool IsStreaming { get; }

    /// <summary>
    /// For non-streaming content, the representative CLR type returned by the endpoint
    /// (for example <see cref="IAsyncEnumerable{T}"/> when streaming vs a collection type when not).
    /// May be null when not applicable.
    /// </summary>
    Type? NoStreamingType { get; }
}