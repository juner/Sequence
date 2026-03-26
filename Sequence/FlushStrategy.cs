namespace Juner.Sequence;

/// <summary>
/// Specifies how flushing is performed during serialization.
/// </summary>
public enum FlushStrategy
{
    /// <summary>
    /// Does not flush after each record.
    /// Flushing is controlled by the caller or underlying transport.
    /// </summary>
    None = 0,

    /// <summary>
    /// Calls Flush after each record is written.
    /// </summary>
    PerRecord = 1,
}