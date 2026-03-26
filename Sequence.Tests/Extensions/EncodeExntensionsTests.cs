using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.Sequence.Extensions;

[TestClass]
public class EncodeExtensionsTests
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
    public async Task Deserialize_With_Encoding()
    {
        var json = "{\"Id\":1,\"Name\":\"あ\"}\n";

        var bytes = Encoding.Unicode.GetBytes(json);
        await using var stream = new MemoryStream(bytes);
        stream.Seek(0, SeekOrigin.Begin);

        var reader = PipeReader.Create(stream);

        var results = new List<TestData>();

        await foreach (var item in SequenceSerializerEncodeExntensions.DeserializeAsyncEnumerable(
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
            await foreach (var _ in SequenceSerializerEncodeExntensions.DeserializeAsyncEnumerable(
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