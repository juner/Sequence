using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence;

[TestClass]
public class SequenceSerializer_DeserializeTests
{
    public required TestContext TestContext { get; set; }
    CancellationToken CancellationToken =>
#if NET8_0_OR_GREATER
        TestContext.CancellationToken;
#else
        TestContext.CancellationTokenSource.Token;
#endif

    static JsonTypeInfo<TestData> GetTypeInfo()
    {
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
        return (JsonTypeInfo<TestData>)serializerOptions.GetTypeInfo(typeof(TestData));
    }

    [TestMethod]
    public async Task JsonLines_Should_Deserialize_Multiple_Items()
    {
        var reader = PipeHelper.CreateReader(
            "{\"Id\":1,\"Name\":\"A\"}\n",
            "{\"Id\":2,\"Name\":\"B\"}\n"
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
        Assert.AreEqual(1, results[0].Id);
        Assert.AreEqual("B", results[1].Name);
    }

    [TestMethod]
    public async Task JsonSequence_Should_Deserialize()
    {
        var rs = "\u001e";

        var reader = PipeHelper.CreateReader(
            $"{rs}{{\"Id\":1,\"Name\":\"A\"}}\n",
            $"{rs}{{\"Id\":2,\"Name\":\"B\"}}\n"
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonSequence,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
    }

    [TestMethod]
    public async Task Should_Handle_Split_Chunks()
    {
        var json = "{\"Id\":1,\"Name\":\"Alice\"}\n";

        // わざと分割
        var reader = PipeHelper.CreateReader(
            json[..10],
            json[10..]
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(1, results);
        Assert.AreEqual("Alice", results[0].Name);
    }

    [TestMethod]
    public async Task Serialize_Then_Deserialize_Should_Roundtrip()
    {
        var pipe = new Pipe();

        static async IAsyncEnumerable<TestData> Source()
        {
            yield return new TestData(1, "A");
            yield return new TestData(2, "B");
        }

        var typeInfo = GetTypeInfo();

        await SequenceSerializer.SerializeAsync(
            pipe.Writer,
            Source(),
            typeInfo,
            SequenceSerializerOptions.JsonLines,
            CancellationToken);

        await pipe.Writer.CompleteAsync();

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            pipe.Reader,
            typeInfo,
            SequenceSerializerOptions.JsonLines,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
        Assert.AreEqual(1, results[0].Id);
        Assert.AreEqual("A", results[0].Name);
        Assert.AreEqual(2, results[1].Id);
        Assert.AreEqual("B", results[1].Name);
    }

    [TestMethod]
    public async Task Empty_Input_Should_Return_Empty()
    {
        var reader = PipeHelper.CreateReader();

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task Should_Use_TryReadFrameAny_With_MultiDelimiter()
    {
        IReadOnlyList<ReadOnlyMemory<byte>> start = [
            Encoding.UTF8.GetBytes("<<").AsMemory(),
            Encoding.UTF8.GetBytes("[[").AsMemory()
        ];

        IReadOnlyList<ReadOnlyMemory<byte>> end = [
            Encoding.UTF8.GetBytes(">>").AsMemory(),
            Encoding.UTF8.GetBytes("]]").AsMemory()
        ];

        var options = new SequenceSerializerOptions(start, end, default, default);

        var reader = PipeHelper.CreateReader(
            "<<{\"Id\":1,\"Name\":\"A\"}>>",
            "[[{\"Id\":2,\"Name\":\"B\"}]]"
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            options,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
    }

    [TestMethod]
    public async Task Should_Handle_Delimiter_Split_Across_Chunks()
    {
        var reader = PipeHelper.CreateReader(
            "{\"Id\":1,\"Name\":\"A\"}",
            "\n{\"Id\":2,\"Name\":\"B\"}\n"
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
    }

    [TestMethod]
    public async Task Should_Read_Last_Frame_Without_EndDelimiter()
    {
        var reader = PipeHelper.CreateReader(
            "{\"Id\":1,\"Name\":\"A\"}\n",
            "{\"Id\":2,\"Name\":\"B\"}" // ← LFなし
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
    }

    [TestMethod]
    public async Task Should_Work_With_No_Start_Delimiter()
    {
        var reader = PipeHelper.CreateReader(
            "{\"Id\":1,\"Name\":\"A\"}\n"
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(1, results);
    }

    [TestMethod]
    public async Task Should_Skip_When_Start_Not_Matched()
    {
        IReadOnlyList<ReadOnlyMemory<byte>> start = [Encoding.UTF8.GetBytes("##").AsMemory()];
        IReadOnlyList<ReadOnlyMemory<byte>> end = [Encoding.UTF8.GetBytes("\n").AsMemory()];

        var options = new SequenceSerializerOptions(start, end, default, default);

        var reader = PipeHelper.CreateReader(
            "{\"Id\":1,\"Name\":\"A\"}\n" // ← startなし
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            options,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task Deserialize_With_Encoding()
    {
        var json = "{\"Id\":1,\"Name\":\"あ\"}\n";

        var bytes = Encoding.Unicode.GetBytes(json);
        await using var stream = new MemoryStream(bytes);
        stream.Seek(0, SeekOrigin.Begin);

        var reader = PipeReader.Create(stream);

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines,
            Encoding.Unicode,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(1, results);
        Assert.AreEqual("あ", results[0].Name);
    }

    [TestMethod]
    public async Task Should_Throw_On_Invalid_Json()
    {
        var reader = PipeHelper.CreateReader(
            "{invalid json}\n"
        );

        await Assert.ThrowsAsync<JsonException>(async () =>
        {
            await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
                reader,
                GetTypeInfo(),
                SequenceSerializerOptions.JsonLines,
                CancellationToken))
            {
            }
        });
    }

    [TestMethod]
    public async Task Should_Not_Stall_On_Incomplete_Frame()
    {
        var reader = PipeHelper.CreateReader(
            "{\"Id\":1" // ← 不完全
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            SequenceSerializerOptions.JsonLines with
            {
                IgnoreIncompleteFrame = true,
            },
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task Should_Handle_Partial_Delimiter_Match()
    {
        IReadOnlyList<ReadOnlyMemory<byte>> end = [
            Encoding.UTF8.GetBytes("ab").AsMemory(),
            Encoding.UTF8.GetBytes("abc").AsMemory()
        ];

        var options = new SequenceSerializerOptions([], end, default, default);

        var reader = PipeHelper.CreateReader(
            "{\"Id\":1}abc"
        );

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            GetTypeInfo(),
            options,
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(1, results);
    }

    [TestMethod]
    public async Task Should_Handle_Large_Stream()
    {
        var pipe = new Pipe();

        static async IAsyncEnumerable<TestData> Source()
        {
            for (var i = 0; i < 10000; i++)
                yield return new TestData(i, "A");
        }

        var typeInfo = GetTypeInfo();
        async Task SerializeAsync(CancellationToken CancellationToken)
        {
            await SequenceSerializer.SerializeAsync(
                pipe.Writer,
                Source(),
                typeInfo,
                SequenceSerializerOptions.JsonLines,
                CancellationToken);

            await pipe.Writer.CompleteAsync();
        }

        var count = 0;
        async Task DeserializeAsync(CancellationToken CancellationToken)
        {
            await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
                pipe.Reader,
                typeInfo,
                SequenceSerializerOptions.JsonLines,
                CancellationToken))
            {
                count++;
            }
        }
        await Task.WhenAll([
            SerializeAsync(CancellationToken),
            DeserializeAsync(CancellationToken),
        ]);

        Assert.AreEqual(10000, count);
    }

    [TestMethod]
    public async Task Should_Propagate_Exception_From_Stream()
    {
        // Readで例外出すやつ
        await using var stream = new ThrowingStream
        {
            ThrowIsRead = true,
        };
        var reader = PipeReader.Create(stream);
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
                reader,
                GetTypeInfo(),
                SequenceSerializerOptions.JsonLines,
                Encoding.Unicode,
                CancellationToken))
            {
            }
        });
    }
}