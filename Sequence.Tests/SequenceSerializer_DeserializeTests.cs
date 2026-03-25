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
}
