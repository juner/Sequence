using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence;

[TestClass]
public class SequenceSerializer_SerializeTests
{
    public required TestContext TestContext { get; set; }
    CancellationToken CancellationToken =>
#if NET8_0_OR_GREATER
        TestContext.CancellationToken;
#else
        TestContext.CancellationTokenSource.Token;
#endif

    static readonly JsonTypeInfo<int> TypeInfo = (JsonTypeInfo<int>)JsonSerializerOptions.Default.GetTypeInfo(typeof(int));

    [TestMethod]
    public async Task SerializeAsync_Stream_JsonLines()
    {
        var stream = new MemoryStream();

        static async IAsyncEnumerable<int> Data()
        {
            yield return 1;
            yield return 2;
        }

        await SequenceSerializer.SerializeAsync(
            stream,
            Data(),
            TypeInfo,
            SequenceSerializerOptions.JsonLines,
            CancellationToken);

        var result = Encoding.UTF8.GetString(stream.ToArray());

        Assert.AreEqual("1\n2\n", result);
    }

    [TestMethod]
    public async Task SerializeAsync_Stream_JsonSequence()
    {
        var stream = new MemoryStream();

        static async IAsyncEnumerable<int> Data()
        {
            yield return 1;
            yield return 2;
        }

        await SequenceSerializer.SerializeAsync(
            stream,
            Data(),
            TypeInfo,
            SequenceSerializerOptions.JsonSequence,
            CancellationToken);

        var result = Encoding.UTF8.GetString(stream.ToArray());

        Assert.AreEqual("\u001e1\n\u001e2\n", result);
    }

    [TestMethod]
    public async Task SerializeAsync_Stream_EmptyOptions()
    {
        var stream = new MemoryStream();

        static async IAsyncEnumerable<int> Data()
        {
            yield return 1;
            yield return 2;
        }

        await SequenceSerializer.SerializeAsync(
            stream,
            Data(),
            TypeInfo,
            SequenceSerializerOptions.Empty,
            CancellationToken);

        var result = Encoding.UTF8.GetString(stream.ToArray());

        Assert.AreEqual("12", result);
    }

    [TestMethod]
    public async Task SerializeAsync_Stream_FlushPerRecord()
    {
        var stream = new TrackingStream();

        static async IAsyncEnumerable<int> Data()
        {
            yield return 1;
            yield return 2;
        }

        var options = SequenceSerializerOptions.JsonLines with
        {
            FlushStrategy = FlushStrategy.PerRecord
        };

        await SequenceSerializer.SerializeAsync(stream, Data(), TypeInfo, options, CancellationToken);

        Assert.AreEqual(2, stream.FlushCount);
    }

    [TestMethod]
    public async Task SerializeAsync_Stream_FlushNone()
    {
        var stream = new TrackingStream();

        static async IAsyncEnumerable<int> Data()
        {
            yield return 1;
            yield return 2;
        }

        var options = SequenceSerializerOptions.JsonLines with
        {
            FlushStrategy = FlushStrategy.None
        };

        await SequenceSerializer.SerializeAsync(stream, Data(), TypeInfo, options, CancellationToken);

        Assert.AreEqual(1, stream.FlushCount); // ← 最後だけ
    }

    [TestMethod]
    public async Task SerializeAsync_PipeWriter_JsonLines()
    {
        var stream = new MemoryStream();
        var writer = PipeWriter.Create(stream);

        static async IAsyncEnumerable<int> Data()
        {
            yield return 1;
            yield return 2;
        }

        await SequenceSerializer.SerializeAsync(
            writer,
            Data(),
            TypeInfo,
            SequenceSerializerOptions.JsonLines,
            CancellationToken);

        await writer.CompleteAsync();

        var result = Encoding.UTF8.GetString(stream.ToArray());

        Assert.AreEqual("1\n2\n", result);
    }

    [TestMethod]
    public async Task SerializeAsync_WithEncoding()
    {
        var stream = new MemoryStream();
        var writer = PipeWriter.Create(stream);

        static async IAsyncEnumerable<string> Data()
        {
            yield return "あ";
        }

        var typeInfo = (JsonTypeInfo<string>)JsonSerializerOptions.Default.GetTypeInfo(typeof(string));

        await SequenceSerializer.SerializeAsync(
            writer,
            Data(),
            typeInfo,
            SequenceSerializerOptions.JsonLines,
            Encoding.Unicode,
            CancellationToken);

        await writer.CompleteAsync();

        var result = Encoding.Unicode.GetString(stream.ToArray());

        Assert.AreEqual("\"\\u3042\"\n", result);
    }

    [TestMethod]
    public async Task SerializeAsync_EmptyEnumerable()
    {
        var stream = new MemoryStream();

        static async IAsyncEnumerable<int> Data()
        {
            yield break;
        }

        await SequenceSerializer.SerializeAsync(
            stream,
            Data(),
            TypeInfo,
            SequenceSerializerOptions.JsonLines,
            CancellationToken);

        Assert.AreEqual(0, stream.Length);
    }
}