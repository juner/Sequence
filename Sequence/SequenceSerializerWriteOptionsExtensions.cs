namespace Juner.Sequence;

public static class SequenceSerializerWriteOptionsExtensions
{
    extension(ISequenceSerializerWriteOptions options)
    {
        /// <summary>
        ///  sequence write options is Invalid
        /// </summary>
        public bool IsInvalid => options is not ({ Start.IsEmpty: false } or { End.IsEmpty: false });
    }
}