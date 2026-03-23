namespace Juner.Sequence;

public static class SequenceSerializerWriteOptionsExtensions
{
    extension(ISequenceSerializerWriteOptions options)
    {
        public bool IsEmpty => options is { Start: { IsEmpty: true }, End: { IsEmpty: true } };
    }
}