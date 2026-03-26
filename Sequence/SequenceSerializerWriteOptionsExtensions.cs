namespace Juner.Sequence;

public static class SequenceSerializerWriteOptionsExtensions
{
    extension(ISequenceSerializerWriteOptions options)
    {
        public bool IsEmpty => options is not ({ Start.IsEmpty: false } or { End.IsEmpty: false });
    }
}