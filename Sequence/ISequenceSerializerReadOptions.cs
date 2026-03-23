namespace Juner.Sequence;

/// <summary>
/// 
/// </summary>
public interface ISequenceSerializerReadOptions
{
    /// <summary>
    /// 
    /// </summary>
    ReadOnlyMemory<byte>[] Start { get; }

    /// <summary>
    /// 
    /// </summary>
    ReadOnlyMemory<byte>[] End { get; }

}