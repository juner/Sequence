namespace Juner.Sequence;

public static class SequenceSerializerReadOptionsExtensions
{
    extension(ISequenceSerializerReadOptions options)
    {
        /// <summary>
        /// sequence read options is Invalid
        /// </summary>
        public bool IsInvalid => options is not ({ Start.Count: > 0 } or { End.Count: > 0 });
    }
}