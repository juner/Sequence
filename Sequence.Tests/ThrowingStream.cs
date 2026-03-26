namespace Juner.Sequence;

class ThrowingStream : MemoryStream
{
    public ThrowingStream() : base() { }
    public ThrowingStream(byte[] buffer) : base(buffer) { }
    public ThrowingStream(int capacity) : base(capacity) { }
    public ThrowingStream(byte[] buffer, bool writable) : base(buffer, writable) { }
    public ThrowingStream(byte[] buffer, int index, int count) : base(buffer, index, count) { }
    public ThrowingStream(byte[] buffer, int index, int count, bool writable) : base(buffer, index, count, writable) { }
    public ThrowingStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible) : base(buffer, index, count, writable, publiclyVisible) { }

    public bool ThrowIsWrite { get; init; } = false;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (ThrowIsWrite) throw new Exception("Write failed");
        base.Write(buffer, offset, count);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (ThrowIsWrite) throw new Exception("Write failed");
        base.Write(buffer);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (ThrowIsWrite) throw new Exception("Write failed");
        return base.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (ThrowIsWrite) throw new Exception("Write failed");
        return base.WriteAsync(buffer, cancellationToken);
    }

    public override void WriteByte(byte value)
    {
        if (ThrowIsWrite) throw new Exception("Write failed");
        base.WriteByte(value);
    }

    public override void WriteTo(Stream stream)
    {
        if (ThrowIsWrite) throw new Exception("Write failed");
        base.WriteTo(stream);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        if (ThrowIsWrite) throw new Exception("Write failed");
        return base.BeginWrite(buffer, offset, count, callback, state);
    }

    public bool ThrowIsRead { get; init; } = false;

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (ThrowIsRead) throw new Exception("Write failed");
        return base.Read(buffer, offset, count);
    }

    public override int Read(Span<byte> buffer)
    {
        if (ThrowIsRead) throw new Exception("Write failed");
        return base.Read(buffer);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (ThrowIsRead) throw new Exception("Write failed");
        return base.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (ThrowIsRead) throw new Exception("Write failed");
        return base.ReadAsync(buffer, cancellationToken);
    }

    public override int ReadByte()
    {
        if (ThrowIsRead) throw new Exception("Write failed");
        return base.ReadByte();
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        if (ThrowIsRead) throw new Exception("Write failed");
        return base.BeginRead(buffer, offset, count, callback, state);
    }

    public bool ThrowIsDispose { get; init; } = false;

    protected override void Dispose(bool disposing)
    {
        if (ThrowIsDispose) throw new Exception("Disposa failed");
        base.Dispose(disposing);
    }
}