namespace Juner.Sequence;

class TrackingStream : MemoryStream
{
    readonly MemoryStream stream;
    public TrackingStream() : base() => stream = new();
    public TrackingStream(byte[] buffer) : base() => stream = new(buffer);
    public TrackingStream(int capacity) : base() => stream = new(capacity);
    public TrackingStream(byte[] buffer, bool writable): base() => stream = new(buffer, writable);
    public TrackingStream(byte[] buffer, int index, int count): base() => stream = new(buffer, index, count);
    public TrackingStream(byte[] buffer, int index, int count, bool writable): base() => stream = new(buffer, index, count, writable);
    public TrackingStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible): base() => stream = new(buffer, index, count, writable, publiclyVisible);
    public override bool CanRead => stream.CanRead;
    public override bool CanSeek => stream.CanSeek;
    public override bool CanTimeout => stream.CanTimeout;
    public override bool CanWrite => stream.CanWrite;
    public override int Capacity { get => stream.Capacity; set => stream.Capacity = value; }
    public override long Length => stream.Length;
    public override long Position { get => stream.Position; set => stream.Position = value; }
    public override int ReadTimeout { get => stream.ReadTimeout; set => stream.ReadTimeout = value; }
    public override int WriteTimeout { get => stream.WriteTimeout; set => stream.WriteTimeout = value; }
    public override long Seek(long offset, SeekOrigin loc) => stream.Seek(offset, loc);
    public override void SetLength(long value) => stream.SetLength(value);

    #region Flush
    public int FlushCount { get; private set; }
    public override void Flush()
    {
        FlushCount++;
        stream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        FlushCount++;
        return stream.FlushAsync(cancellationToken);
    }
    #endregion

    #region Read
    double readCount = 0;
    public int ReadCount => (int)readCount;
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        readCount += 0.5;
        return stream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        readCount += 0.5;
        return stream.EndRead(asyncResult);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        readCount++;
        return stream.Read(buffer, offset, count);
    }

    public override int Read(Span<byte> buffer)
    {
        readCount++;
        return stream.Read(buffer);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        readCount++;
        return stream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        readCount++;
        return stream.ReadAsync(buffer, cancellationToken);
    }

    public override int ReadByte()
    {
        readCount++;
        return stream.ReadByte();
    }
    #endregion

    #region Write

    double writeCount = 0;
    public int WriteCount => (int)writeCount;
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        writeCount += 0.5;
        return stream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        writeCount += 0.5;
        stream.EndWrite(asyncResult);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        writeCount++;
        stream.Write(buffer, offset, count);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        writeCount++;
        stream.Write(buffer);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        writeCount++;
        return stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        writeCount++;
        return stream.WriteAsync(buffer, cancellationToken);
    }

    public override void WriteByte(byte value)
    {
        writeCount++;
        stream.WriteByte(value);
    }
    #endregion

    public override void Close() => stream.Close();
    public override void CopyTo(Stream destination, int bufferSize) => stream.CopyTo(destination, bufferSize);
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => stream.CopyToAsync(destination, bufferSize, cancellationToken);
    public override async ValueTask DisposeAsync()
    {
        await stream.DisposeAsync();
        await base.DisposeAsync();
    }
    protected override void Dispose(bool disposing)
    {
        stream.Dispose();
        base.Dispose(disposing);
    }
    public override bool Equals(object? obj) => stream.Equals(obj);
    public override bool TryGetBuffer(out ArraySegment<byte> buffer) => stream.TryGetBuffer(out buffer);
    public override byte[] ToArray() => stream.ToArray();
    public override string ToString() => stream.ToString()!;
    public override byte[] GetBuffer() => stream.GetBuffer();
    public override int GetHashCode() => stream.GetHashCode();

}