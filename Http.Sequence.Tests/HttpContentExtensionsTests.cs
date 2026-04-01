using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Juner.Sequence;

namespace Juner.Http.Sequence;

[TestClass]
public sealed class HttpContentExtensionsTests
{
    public required TestContext TestContext { get; set; }
    CancellationToken CancellationToken =>
#if NET8_0_OR_GREATER
        TestContext.CancellationToken;
#else
        TestContext.CancellationTokenSource.Token;
#endif

    static JsonTypeInfo<T> GetTypeInfo<T>() => (JsonTypeInfo<T>)JsonSerializerOptions.Default.GetTypeInfo(typeof(T));

    [TestMethod]
    public async Task ReadJsonLinesAsyncEnumerable_Should_Read_Items()
    {
        var json = """
        {"Id":1}
        {"Id":2}
        """;

        var content = new StringContent(json);
        var results = new List<TestData>();

        await foreach (var item in content.ReadJsonLinesAsyncEnumerable(
            GetTypeInfo<TestData>(),
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
        Assert.AreEqual(1, results[0].Id);
        Assert.AreEqual(2, results[1].Id);
    }

    [TestMethod]
    public async Task ReadJsonSequenceAsyncEnumerable_Should_Read_Items()
    {
        var rs = (char)0x1E;
        var json = $"{rs}{{\"Id\":1}}{rs}{{\"Id\":2}}";

        var content = new StringContent(json);
        var results = new List<TestData>();

        await foreach (var item in content.ReadJsonSequenceAsyncEnumerable(
            GetTypeInfo<TestData>(),
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
    }

    [TestMethod]
    public void ReadSequenceEnumerable_Should_Throw_When_Options_Empty()
    {
        var content = new StringContent("");

        Assert.Throws<ArgumentException>(() =>
        {
            content.ReadSequenceEnumerable(
                GetTypeInfo<TestData>(),
                new SequenceSerializerOptions([], [], default, default)
            );
        });
    }

    static async IAsyncEnumerable<T> GetAsyncEnumerable<T>(params T[] values)
    {
        foreach (var value in values)
            yield return value;
    }
    [TestMethod]
    public async Task WithJsonLinesContent_Should_RoundTrip()
    {
        var source = GetAsyncEnumerable(
            new TestData(1),
            new TestData(2)
        );

        var request = new HttpRequestMessage()
            .WithJsonLinesContent(source, GetTypeInfo<TestData>(), CancellationToken);

        var stream = await request.Content!.ReadAsStreamAsync(TestContext.CancellationTokenSource.Token);
        var content = new StreamContent(stream);

        var results = new List<TestData>();

        await foreach (var item in content.ReadJsonLinesAsyncEnumerable(
            GetTypeInfo<TestData>(),
            CancellationToken))
        {
            results.Add(item);
        }

        Assert.HasCount(2, results);
    }

    [TestMethod]
    public void WithJsonSequenceContent_Should_Set_ContentType()
    {
        var request = new HttpRequestMessage()
            .WithJsonSequenceContent(GetAsyncEnumerable<TestData>(), GetTypeInfo<TestData>());

        Assert.AreEqual("application/json-seq",
            request.Content!.Headers.ContentType!.MediaType);
    }

    [TestMethod]
    public async Task SequenceHttpContent_Should_Write_To_Stream()
    {
        var source = GetAsyncEnumerable(
            new TestData(1)
        );

        var content = new SequenceHttpContent<TestData>(
            source,
            GetTypeInfo<TestData>(),
            "application/jsonl",
            SequenceSerializerOptions.JsonLines
        );

        using var ms = new MemoryStream();
        await content.CopyToAsync(ms);

        var text = Encoding.UTF8.GetString(ms.ToArray());

        Assert.Contains("\"Id\":1", text);
    }

    [TestMethod]
    public async Task SequenceHttpContent_Should_Not_Compute_Length()
    {
        var content = new SequenceHttpContent<TestData>(
            GetAsyncEnumerable<TestData>(new TestData(1)),
            GetTypeInfo<TestData>(),
            "application/jsonl",
            SequenceSerializerOptions.JsonLines
        );

        using var ms = new MemoryStream();

        await content.CopyToAsync(ms);

        Assert.IsNull(content.Headers.ContentLength);
    }
}