namespace Juner.Sequence;

/// <summary>
/// 
/// </summary>
public interface ISequenceSerializerWriteOptions
{
    /// <summary>
    /// 
    /// </summary>
    ReadOnlyMemory<byte> Start { get; }

    /// <summary>
    /// 
    /// </summary>
    ReadOnlyMemory<byte> End { get; }
}