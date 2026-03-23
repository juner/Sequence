namespace Juner.Sequence;

public interface ISequenceSerializerOptions
{
    ReadOnlyMemory<byte>[] Start { get; }
    ReadOnlyMemory<byte>[] End { get; }

}
public record SequenceSerializerOptions(ReadOnlyMemory<byte>[] Start, ReadOnlyMemory<byte>[] End)
{
    static readonly byte[] RS = [.. "\u001e"u8];
    static readonly byte[] LF = [.. "\n"u8];

    static readonly ReadOnlyMemory<byte>[] JSONSEQ_START = [RS];
    static readonly ReadOnlyMemory<byte>[] JSONSEQ_END = [LF];

    static readonly ReadOnlyMemory<byte>[] NDJSON_START = [];
    static readonly ReadOnlyMemory<byte>[] NDJSON_END = [LF];

    static SequenceSerializerOptions? _jsonSequence;
    public static ISequenceSerializerOptions JsonSequence
    {
        get
        {
            if (_jsonSequence is null)
            {
                Interlocked.CompareExchange(ref _jsonSequence, new SequenceSerializerOptions(JSONSEQ_START, JSONSEQ_END), null);
            }
            return _jsonSequence;
        }

    }
}
