using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
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
}

file static class SequenceSerializer_Deserialize
{

    /// <summary>
    /// Delegate used to attempt to read a single frame from the input buffer.
    /// Returns true when a complete frame is produced in <paramref name="frame"/>.
    /// </summary>
    /// <param name="buffer">The input buffer to read from. This will be sliced when a frame is consumed.</param>
    /// <param name="start">List of possible start delimiters.</param>
    /// <param name="end">List of possible end delimiters.</param>
    /// <param name="frame">The resulting frame when the delegate returns true.</param>
    /// <returns>True if a frame was found; otherwise false.</returns>
    public delegate bool TryReadFrame(
        ref ReadOnlySequence<byte> buffer,
        IReadOnlyList<ReadOnlyMemory<byte>> start,
        IReadOnlyList<ReadOnlyMemory<byte>> end,
        out ReadOnlySequence<byte> frame);

    /// <summary>
    /// Try to read a frame terminated by a single end byte.
    /// </summary>
    /// <param name="buffer">The input buffer to scan. Advances past the delimiter on success.</param>
    /// <param name="_">Unused start delimiters.</param>
    /// <param name="end">The end delimiter list (expected to contain one single-byte delimiter).</param>
    /// <param name="frame">The extracted frame (excluding the delimiter) when successful.</param>
    /// <returns>True if a frame was found; otherwise false.</returns>
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

    /// <summary>
    /// Try to read a frame which starts with a single start byte and ends with a single end byte.
    /// </summary>
    /// <param name="buffer">The input buffer to scan.</param>
    /// <param name="start">The start delimiter list (expected to contain one single-byte delimiter).</param>
    /// <param name="end">The end delimiter list (expected to contain one single-byte delimiter).</param>
    /// <param name="frame">The extracted frame (excluding delimiters) when successful.</param>
    /// <returns>True if a frame was found; otherwise false.</returns>
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

    /// <summary>
    /// Try to read a frame using arbitrary multi-byte start/end delimiters.
    /// </summary>
    /// <param name="buffer">The input buffer to scan.</param>
    /// <param name="start">The list of possible start delimiters.</param>
    /// <param name="end">The list of possible end delimiters.</param>
    /// <param name="frame">The extracted frame (excluding delimiters) when successful.</param>
    /// <returns>True if a frame was found; otherwise false.</returns>
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

    /// <summary>
    /// Check whether the reader is positioned at any of the provided start delimiters.
    /// Advances the reader past the matching start delimiter when found.
    /// </summary>
    /// <param name="reader">The sequence reader to inspect.</param>
    /// <param name="start">The list of possible start delimiters.</param>
    /// <returns>True if a start delimiter was matched; otherwise false.</returns>
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

    /// <summary>
    /// Attempt to read until any of the provided delimiters is encountered.
    /// Performs a longest-match selection when multiple delimiters share the same starting byte.
    /// </summary>
    /// <param name="reader">The sequence reader to scan from.</param>
    /// <param name="delimiters">List of possible end delimiters.</param>
    /// <param name="frame">The extracted frame (excluding the delimiter) when successful.</param>
    /// <returns>True if a delimiter and frame were found; otherwise false.</returns>
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

    /// <summary>
    /// Attempt to read the final frame from the buffer when no end delimiter is present.
    /// The method checks for a valid start delimiter and then returns the remaining buffer as a frame.
    /// </summary>
    /// <param name="buffer">The input buffer to read from.</param>
    /// <param name="start">List of possible start delimiters.</param>
    /// <param name="frame">The remaining buffer that represents the last frame.</param>
    /// <returns>True if a final frame was extracted; otherwise false.</returns>
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