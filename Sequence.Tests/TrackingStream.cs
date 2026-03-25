namespace Juner.Sequence;

class TrackingStream : MemoryStream
{
    public int FlushCount { get; private set; }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        FlushCount++;
        return base.FlushAsync(cancellationToken);
    }
}