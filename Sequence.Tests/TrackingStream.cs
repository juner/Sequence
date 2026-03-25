namespace Juner.Sequence;

class TrackingStream : MemoryStream
{
    public int FlushCount { get; private set; }
    bool isflushing = false;
    public override void Flush()
    {
        if (isflushing)
        {
            base.Flush();
            return;
        }
        FlushCount++;
        isflushing = true;
        try {        
            base.Flush();
        }finally
        {
            isflushing = false;
        }
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        if (isflushing)
        {
            await base.FlushAsync(cancellationToken);
            return;
        }
        isflushing = true;
        FlushCount++;
        try {
            await base.FlushAsync(cancellationToken);
        }
        finally
        {
            isflushing = false;
        }
    }
}