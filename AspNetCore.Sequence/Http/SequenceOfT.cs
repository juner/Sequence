using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Juner.AspNetCore.Sequence.Http;

public sealed partial class Sequence<T> : IAsyncEnumerable<T>, IEndpointParameterMetadataProvider
{
    readonly object? _values;
    public Sequence(IAsyncEnumerable<T> values) => _values = values;
    public Sequence(ChannelReader<T> values) => _values = values;
    public Sequence(IEnumerable<T> values) => _values = values;
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => _values switch
        {
            IAsyncEnumerable<T> asyncEnumerable => asyncEnumerable.GetAsyncEnumerator(cancellationToken),
            ChannelReader<T> channelReader => ChunnelReaderAsyncEnumerator(channelReader, cancellationToken),
            IEnumerable<T> enumerable => EnumerableAsyncEnumerator(enumerable),
            _ => EmptyAsyncEnumerator(),
        };

    static async IAsyncEnumerator<T> ChunnelReaderAsyncEnumerator(ChannelReader<T> channelReader, CancellationToken cancellationToken)
    {
        while (await channelReader.WaitToReadAsync(cancellationToken))
            if (channelReader.TryRead(out var item))
                yield return item;
        yield break;
    }

    static async IAsyncEnumerator<T> EnumerableAsyncEnumerator(IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
            yield return item;
    }
    static async IAsyncEnumerator<T> EmptyAsyncEnumerator()
    {
        yield break;
    }

    static partial class Log
    {
        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "not register IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>"
        )]
        public static partial void LogNotHaveJsonOptions(ILogger logger);
    }
}