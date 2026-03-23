namespace Juner.Sequence;

public static class SequenceSerializerReadOptionsExtensions
{
    extension(ISequenceSerializerReadOptions options)
    {
        public bool IsEmpty => options is not ({ Start: { Length: > 0 } } or { End: { Length: > 0 } });
    }
}