using System.Threading.Channels;

using Juner.AspNetCore.Sequence.Http.HttpResults;

namespace Juner.AspNetCore.Sequence.Http;

public static class SequenceResults
{
    /// <summary>
    /// Create a <see cref="JsonSequenceResult{T}"/> that writes the values using the JSON Sequence format.
    /// </summary>
    public static JsonSequenceResult<T> JsonSequence<T>(IAsyncEnumerable<T> values) => new(values);
    /// <summary>
    /// Create a <see cref="JsonSequenceResult{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static JsonSequenceResult<T> JsonSequence<T>(IEnumerable<T> values) => new(values);
    /// <summary>
    /// Create a <see cref="JsonSequenceResult{T}"/> from a <see cref="ChannelReader{T}"/>.
    /// </summary>
    public static JsonSequenceResult<T> JsonSequence<T>(ChannelReader<T> values) => new(values);

    /// <summary>
    /// Create a <see cref="JsonLineResult{T}"/> that writes newline-delimited JSON lines.
    /// </summary>
    public static JsonLineResult<T> JsonLine<T>(IAsyncEnumerable<T> values) => new(values);
    /// <summary>
    /// Create a <see cref="JsonLineResult{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static JsonLineResult<T> JsonLine<T>(IEnumerable<T> values) => new(values);
    /// <summary>
    /// Create a <see cref="JsonLineResult{T}"/> from a <see cref="ChannelReader{T}"/>.
    /// </summary>
    public static JsonLineResult<T> JsonLine<T>(ChannelReader<T> values) => new(values);

    /// <summary>
    /// Create a <see cref="NdJsonResult{T}"/> that writes NDJSON (newline-delimited JSON) payloads.
    /// </summary>
    public static NdJsonResult<T> NdJson<T>(IAsyncEnumerable<T> values) => new(values);
    /// <summary>
    /// Create a <see cref="NdJsonResult{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static NdJsonResult<T> NdJson<T>(IEnumerable<T> values) => new(values);
    /// <summary>
    /// Create a <see cref="NdJsonResult{T}"/> from a <see cref="ChannelReader{T}"/>.
    /// </summary>
    public static NdJsonResult<T> NdJson<T>(ChannelReader<T> values) => new(values);

    /// <summary>
    /// Create a generic sequence result that may use the provided content type when writing.
    /// </summary>
    public static SequenceResult<T> Sequence<T>(IAsyncEnumerable<T> values, string? contentType = null) => new(values, contentType);
    /// <summary>
    /// Create a generic sequence result from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static SequenceResult<T> Sequence<T>(IEnumerable<T> values, string? contentType = null) => new(values, contentType);
    /// <summary>
    /// Create a generic sequence result from a <see cref="ChannelReader{T}"/>.
    /// </summary>
    public static SequenceResult<T> Sequence<T>(ChannelReader<T> values, string? contentType = null) => new(values, contentType);

}