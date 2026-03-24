using Juner.Sequence;
using System.IO.Pipelines;
using System.Net;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Http.Sequence;

public sealed class SequenceHttpContent<T> : HttpContent
{
    readonly IAsyncEnumerable<T> _source;
    readonly JsonTypeInfo<T> _typeInfo;
    readonly ISequenceSerializerWriteOptions _options;
    readonly CancellationToken _cancellationToken;

    public SequenceHttpContent(
        IAsyncEnumerable<T> source,
        JsonTypeInfo<T> typeInfo,
        string contentType,
        ISequenceSerializerWriteOptions options,
        CancellationToken cancellationToken = default)
    {
        _source = source;
        _typeInfo = typeInfo;
        _options = options;
        _cancellationToken = cancellationToken;

        Headers.ContentType = new(contentType);
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
     => await SequenceSerializer.SerializeAsync(
            PipeWriter.Create(stream),
            _source,
            _typeInfo,
            _options,
            _cancellationToken);

    protected override bool TryComputeLength(out long length)
    {
        length = -1;
        return false;
    }
}