using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using static Juner.Sequence.SequenceSerializer_Deserialize;

namespace Juner.Sequence;

public static partial class SequenceSerializer
{
    /// <summary>
    /// deserialize sequence format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerReadOptions options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var start = options.Start;
        var end = options.End;
        TryReadFrame tryReadFrame = (start, end) switch
        {
            ({ Count: 0 }, [{ Length: 1 }]) => TryReadFrameEndByteOnly,
            ([{ Length: 1 }], [{ Length: 1 }]) => TryReadFrameStartEndByteOnly,
            _ => TryReadFrameAny,
        };
        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);
            var buffer = result.Buffer;

            while (tryReadFrame(ref buffer, start, end, out var frame))
            {
                if (frame.IsEmpty) continue;
                Utf8JsonReader jsonReader = frame is { IsSingleSegment: true } ? new(frame.FirstSpan) : new(frame);
                T? value;
                try
                {
                    value = JsonSerializer.Deserialize(ref jsonReader, jsonTypeInfo);
                }
                catch (JsonException) when (options.IgnoreIncompleteFrame)
                {
                    yield break;
                }

                if (value is not null)
                    yield return value;
            }

            if (result.IsCompleted)
            {
                if (!buffer.IsEmpty)
                {
                    if (TryReadLastFrame(ref buffer, start, out var frame))
                    {
                        if (frame.IsEmpty) yield break;
                        var jsonReader = new Utf8JsonReader(frame);
                        T? value;
                        try
                        {
                            value = JsonSerializer.Deserialize(ref jsonReader, jsonTypeInfo);
                        }
                        catch (JsonException) when (options.IgnoreIncompleteFrame)
                        {
                            yield break;
                        }
                        if (value is not null)
                            yield return value;
                    }
                }

                reader.AdvanceTo(buffer.End);
                yield break;
            }

            reader.AdvanceTo(buffer.Start, buffer.End);
        }
    }

    /// <summary>
    /// deserialize sequence format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="options"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IAsyncEnumerable<T> DeserializeAsyncEnumerable<T>(PipeReader reader, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerReadOptions options, Encoding? encoding, CancellationToken cancellationToken = default)
    {
        encoding ??= Encoding.UTF8;
        if (Encoding.UTF8 == encoding)
        {
            return DeserializeAsyncEnumerable(reader, jsonTypeInfo, options, cancellationToken);
        }
        return WrappedDeserializeAsyncEnumerable(Encoding.CreateTranscodingStream(reader.AsStream(), encoding, Encoding.UTF8, leaveOpen: true), jsonTypeInfo, options, cancellationToken);
        static async IAsyncEnumerable<T> WrappedDeserializeAsyncEnumerable(Stream stream, JsonTypeInfo<T> jsonTypeInfo, ISequenceSerializerReadOptions options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var reader = PipeReader.Create(stream);
            ExceptionDispatchInfo? exceptionDispatchInfo = null;

            try
            {
                await using var enumerable = DeserializeAsyncEnumerable(reader, jsonTypeInfo, options, cancellationToken).WithCancellation(cancellationToken).GetAsyncEnumerator();
                var next = true;
                while (next)
                {
                    try
                    {
                        next = await enumerable.MoveNextAsync();
                        if (!next) break;
                    }
                    catch (Exception ex)
                    {
                        exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                        break;
                    }
                    yield return enumerable.Current;
                }
            }
            finally
            {
                exceptionDispatchInfo?.Throw();
            }

        }
    }
}

file static class SequenceSerializer_Deserialize
{

    public delegate bool TryReadFrame(
        ref ReadOnlySequence<byte> buffer,
        IReadOnlyList<ReadOnlyMemory<byte>> start,
        IReadOnlyList<ReadOnlyMemory<byte>> end,
        out ReadOnlySequence<byte> frame);

    public static bool TryReadFrameEndByteOnly(
        ref ReadOnlySequence<byte> buffer,
        IReadOnlyList<ReadOnlyMemory<byte>> _,
        IReadOnlyList<ReadOnlyMemory<byte>> end,
        out ReadOnlySequence<byte> frame)
    {
        var e = end[0].Span[0];
        var reader = new SequenceReader<byte>(buffer);

        if (!reader.TryReadTo(out frame, e))
            return false;

        buffer = buffer.Slice(reader.Position);

        return true;
    }

    public static bool TryReadFrameStartEndByteOnly(
         ref ReadOnlySequence<byte> buffer,
         IReadOnlyList<ReadOnlyMemory<byte>> start,
         IReadOnlyList<ReadOnlyMemory<byte>> end,
         out ReadOnlySequence<byte> frame)
    {
        frame = default;
        var s = start[0].Span[0];
        var e = end[0].Span[0];

        var reader = new SequenceReader<byte>(buffer);

        if (!reader.IsNext(s, advancePast: true))
            return false;

        if (!reader.TryReadTo(out frame, e))
            return false;

        buffer = buffer.Slice(reader.Position);

        return true;
    }

    public static bool TryReadFrameAny(
         ref ReadOnlySequence<byte> buffer,
         IReadOnlyList<ReadOnlyMemory<byte>> start,
         IReadOnlyList<ReadOnlyMemory<byte>> end,
         out ReadOnlySequence<byte> frame)
    {
        frame = default;

        var reader = new SequenceReader<byte>(buffer);

        if (!MatchStart(ref reader, start))
            return false;

        if (!TryReadToAny(ref reader, end, out frame))
            return false;

        buffer = buffer.Slice(reader.Position);

        return true;
    }

    public static bool MatchStart(ref SequenceReader<byte> reader, IReadOnlyList<ReadOnlyMemory<byte>> start)
    {
        if (start.Count is 0)
            return true;

        foreach (var s in start)
        {
            if (reader.IsNext(s.Span, advancePast: true))
                return true;
        }

        return false;
    }

    public static bool TryReadToAny(
        ref SequenceReader<byte> reader,
        IReadOnlyList<ReadOnlyMemory<byte>> delimiters,
        out ReadOnlySequence<byte> frame)
    {
        frame = default;

        if (delimiters.Count is 0)
            return false;

        Span<byte> firstBytes = stackalloc byte[delimiters.Count];

        for (var i = 0; i < delimiters.Count; i++)
            firstBytes[i] = delimiters[i].Span[0];

        var start = reader.Position;

        while (reader.TryAdvanceToAny(firstBytes, advancePastDelimiter: false))
        {
            var pos = reader.Position;

            ReadOnlyMemory<byte>? bestMatch = null;

            // 👇 advanceしないで「最長一致」を探す
            foreach (var d in delimiters)
            {
                if (reader.IsNext(d.Span, advancePast: false))
                {
                    if (bestMatch is null || d.Length > bestMatch.Value.Length)
                    {
                        bestMatch = d;
                    }
                }
            }

            if (bestMatch is not null)
            {
                reader.Advance(bestMatch.Value.Length);

                frame = reader.Sequence.Slice(start, pos);
                return true;
            }
            reader.Advance(1);
        }
        return false;
    }

    public static bool TryReadLastFrame(
        ref ReadOnlySequence<byte> buffer,
        IReadOnlyList<ReadOnlyMemory<byte>> start,
        out ReadOnlySequence<byte> frame)
    {
        var reader = new SequenceReader<byte>(buffer);

        if (!MatchStart(ref reader, start))
        {
            frame = default;
            return false;
        }

        frame = buffer.Slice(reader.Position);

        buffer = buffer.Slice(buffer.End);

        return true;
    }
}