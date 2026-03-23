namespace Juner.Sequence;

public class SequenceSerializerOptions(ReadOnlyMemory<byte>[] ReadStart, ReadOnlyMemory<byte>[] ReadEnd, ReadOnlyMemory<byte> WriteStart, ReadOnlyMemory<byte> WriteEnd) : ISequenceSerializerOptions
{
    static readonly byte[] RS = [.. "\u001e"u8];
    static readonly byte[] LF = [.. "\n"u8];

    ReadOnlyMemory<byte>[] ISequenceSerializerReadOptions.Start => ReadStart;
    ReadOnlyMemory<byte>[] ISequenceSerializerReadOptions.End => ReadEnd;

    ReadOnlyMemory<byte> ISequenceSerializerWriteOptions.Start => WriteStart;

    ReadOnlyMemory<byte> ISequenceSerializerWriteOptions.End => WriteEnd;

    static SequenceSerializerOptions? _jsonSequence;
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
    static SequenceSerializerOptions? _jsonLines;
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
    static SequenceSerializerOptions? _empty;
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
}