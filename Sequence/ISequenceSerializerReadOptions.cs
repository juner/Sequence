namespace Juner.Sequence;

/// <summary>
/// sequence serialize read options
/// </summary>
public interface ISequenceSerializerReadOptions
{
    /// <summary>
    /// start sequences by record
    /// </summary>
    IReadOnlyList<ReadOnlyMemory<byte>> Start { get; }

    /// <summary>
    /// end sequences by recrod
    /// </summary>
    IReadOnlyList<ReadOnlyMemory<byte>> End { get; }

    /// <summary>
    /// ignore incomplete frame
    /// </summary>
    bool IgnoreIncompleteFrame { get; }
}