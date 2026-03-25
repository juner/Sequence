namespace Juner.Sequence;

/// <summary>
/// writable sequence options
/// </summary>
public interface ISequenceSerializerWriteOptions
{
    /// <summary>
    /// write start sequence by record
    /// </summary>
    ReadOnlyMemory<byte> Start { get; }

    /// <summary>
    /// write end sequence by record
    /// </summary>
    ReadOnlyMemory<byte> End { get; }

    /// <summary>
    /// Specifies how flushing is performed during serialization.
    /// </summary>
    FlushStrategy FlushStrategy { get; }
}