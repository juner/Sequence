namespace Juner.Sequence;

/// <summary>
/// Represents configuration for sequence-based serialization and deserialization.
/// </summary>
/// <param name="ReadStart"></param>
/// <param name="ReadEnd"></param>
/// <param name="WriteStart"></param>
/// <param name="WriteEnd"></param>
/// <param name="FlushStrategy"></param>
public sealed record SequenceSerializerOptions(
    IReadOnlyList<ReadOnlyMemory<byte>> ReadStart,
    IReadOnlyList<ReadOnlyMemory<byte>> ReadEnd,
    ReadOnlyMemory<byte> WriteStart,
    ReadOnlyMemory<byte> WriteEnd,
    FlushStrategy FlushStrategy = FlushStrategy.PerRecord
) : ISequenceSerializerOptions
{
    static readonly byte[] RS = [.. "\u001e"u8];
    static readonly byte[] LF = [.. "\n"u8];

#pragma warning disable IDE0305 // コレクションの初期化を簡略化します
    public IReadOnlyList<ReadOnlyMemory<byte>> ReadStart { get; } = ReadStart.ToArray();
#pragma warning restore IDE0305 // コレクションの初期化を簡略化します
#pragma warning disable IDE0305 // コレクションの初期化を簡略化します
    public IReadOnlyList<ReadOnlyMemory<byte>> ReadEnd { get; } = ReadEnd.ToArray();
#pragma warning restore IDE0305 // コレクションの初期化を簡略化します

    /// <inheritdoc cref="ISequenceSerializerReadOptions.Start"/>
    IReadOnlyList<ReadOnlyMemory<byte>> ISequenceSerializerReadOptions.Start => ReadStart;

    /// <inheritdoc cref="ISequenceSerializerReadOptions.End"/>
    IReadOnlyList<ReadOnlyMemory<byte>> ISequenceSerializerReadOptions.End => ReadEnd;

    /// <inheritdoc cref="ISequenceSerializerWriteOptions.Start"/>
    ReadOnlyMemory<byte> ISequenceSerializerWriteOptions.Start => WriteStart;

    /// <inheritdoc cref="ISequenceSerializerWriteOptions.End"/>
    ReadOnlyMemory<byte> ISequenceSerializerWriteOptions.End => WriteEnd;

    #region JsonSequence
    static SequenceSerializerOptions? _jsonSequence;

    /// <summary>
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7464">RFC 7464</see> application/json-seq format
    /// </summary>
    public static ISequenceSerializerOptions JsonSequence
    {
        get
        {
            if (_jsonSequence is null)
            {
                Interlocked.CompareExchange(ref _jsonSequence, new SequenceSerializerOptions([RS], [LF], RS, LF), null);
            }
            return _jsonSequence;
        }
    }
    #endregion

    #region JsonLines
    static SequenceSerializerOptions? _jsonLines;

    /// <summary>
    /// <see href="https://jsonlines.org/">JSON Lines</see> (<see href="https://web.archive.org/web/20231218162511/https://ndjson.org/">ndjson</see>), newline-delimited JSON format.
    /// </summary>
    public static ISequenceSerializerOptions JsonLines
    {
        get
        {
            if (_jsonLines is null)
            {
                Interlocked.CompareExchange(ref _jsonLines, new SequenceSerializerOptions([], [LF], default, LF), null);
            }
            return _jsonLines;
        }
    }
    #endregion

    #region Empty
    static SequenceSerializerOptions? _empty;

    /// <summary>
    /// empty option
    /// </summary>
    public static ISequenceSerializerOptions Empty
    {
        get
        {
            if (_empty is null)
            {
                Interlocked.CompareExchange(ref _empty, new SequenceSerializerOptions([], [], default, default), null);
            }
            return _empty;
        }
    }
    #endregion
}