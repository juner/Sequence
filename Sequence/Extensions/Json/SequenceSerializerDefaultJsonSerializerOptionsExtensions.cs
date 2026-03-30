using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text.Json;

namespace Juner.Sequence.Extensions.Json;

/// <summary>
/// <see cref="SequenceSerializer"/> use <see cref="JsonSerializerOptions"/> extensions
/// </summary>
public static class SequenceSerializerDefaultJsonSerializerOptionsExtensions
{
    const string RequiresUnreferencedCodeMessage = "Uses JsonSerializerOptions.Default which may require reflection.";
    const string RequiresDynamicCodeMessage = "May not be AOT compatible.";
    extension(SequenceSerializer)
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="enumerable"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
        [RequiresDynamicCode(RequiresDynamicCodeMessage)]
        public static Task SerializeAsync<T>(PipeWriter writer, IAsyncEnumerable<T> enumerable, ISequenceSerializerWriteOptions options, CancellationToken cancellationToken = default)
            => SequenceSerializerJsonSerializerOptionsExtensions.SerializeAsync(writer, enumerable, JsonSerializerOptions.Default, options, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
        [RequiresDynamicCode(RequiresDynamicCodeMessage)]
        public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, ISequenceSerializerReadOptions options, CancellationToken cancellationToken = default)
            => SequenceSerializerJsonSerializerOptionsExtensions.DeserializeAsyncEnumerable<T>(reader, JsonSerializerOptions.Default, options, cancellationToken);
    }
}