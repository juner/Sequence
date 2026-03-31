using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Buffers;
using System.Text.Json;
using Juner.Sequence;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using Juner.Sequence.Extensions;
using BenchmarkDotNet.Jobs;
using System.IO.Pipelines;

[
    SimpleJob(RuntimeMoniker.Net10_0, launchCount:50 ), 
    SimpleJob(RuntimeMoniker.Net90, launchCount:50 ), 
    SimpleJob(RuntimeMoniker.Net80, launchCount:50), 
    SimpleJob(RuntimeMoniker.Net70, launchCount:50)
]
[MemoryDiagnoser, ]
public class StreamingBenchmarks
{
    private readonly MyType[] _arrayData;
    private readonly IAsyncEnumerable<MyType> _streamData;

    public StreamingBenchmarks()
    {
        _arrayData = Enumerable.Range(0, 100_000)
            .Select(i => new MyType { Id = i, Name = $"Item {i}" })
            .ToArray();

        _streamData = GetStreamData();
    }

    private async IAsyncEnumerable<MyType> GetStreamData()
    {
        foreach (var item in _arrayData)
        {
            yield return item;
            await Task.Yield(); // simulate async source
        }
    }
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        TypeInfoResolver = MyJsonContext.Default,
    };

    private static readonly JsonTypeInfo<MyType> TypeInfo = (JsonTypeInfo<MyType>)jsonSerializerOptions.GetTypeInfo(typeof(MyType));

    // ------------------------------------------------------------
    // 1. Juner.Sequence — NDJSON streaming
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Serialize_NdJson_Streaming()
    {
        await using var stream = new MemoryStream();

        await SequenceSerializer.SerializeAsync(
            stream,
            _streamData,
            TypeInfo,
            SequenceSerializerOptions.JsonLines);
    }

    // ------------------------------------------------------------
    // 2. System.Text.Json — JSON array (non-streaming)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Serialize_JsonArray()
    {
        await using var stream = new MemoryStream();
        JsonSerializer.Serialize(
            stream,
            _arrayData,
            MyJsonContext.Default.MyTypeArray);
    }

    // ------------------------------------------------------------
    // 3. Baseline — pure IAsyncEnumerable iteration
    // ------------------------------------------------------------
    [Benchmark(Baseline = true)]
    public async Task Iterate_IAsyncEnumerable()
    {
        await foreach (var _ in _streamData)
        {
        }
    }

    // ------------------------------------------------------------
    // 4. Juner.Sequence — NDJSON streaming (PipeWriter)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Serialize_NdJson_PipeWriter_Streaming()
    {
        await using var stream = new MemoryStream();
        var writer = PipeWriter.Create(stream);

        await SequenceSerializer.SerializeAsync(
            writer,
            _streamData,
            TypeInfo,
            SequenceSerializerOptions.JsonLines);
    }

    // ------------------------------------------------------------
    // 5. Juner.Sequence — NDJSON Deserialize (Stream)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Deserialize_NdJson_Streaming()
    {
        // Prepare NDJSON payload
        await using var buffer = new MemoryStream();
        await SequenceSerializer.SerializeAsync(
            buffer,
            _streamData,
            TypeInfo,
            SequenceSerializerOptions.JsonLines);

        buffer.Position = 0;

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            buffer,
            TypeInfo,
            SequenceSerializerOptions.JsonLines))
        {
            // consume
        }
    }

    // ------------------------------------------------------------
    // 6. Juner.Sequence — NDJSON Deserialize (PipeReader)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Deserialize_NdJson_PipeReader_Streaming()
    {
        // Prepare NDJSON payload
        await using var buffer = new MemoryStream();
        await SequenceSerializer.SerializeAsync(
            buffer,
            _streamData,
            TypeInfo,
            SequenceSerializerOptions.JsonLines);

        buffer.Position = 0;

        var reader = PipeReader.Create(buffer);

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            TypeInfo,
            SequenceSerializerOptions.JsonLines))
        {
            // consume
        }

        await reader.CompleteAsync();
    }

    // ------------------------------------------------------------
    // 7. System.Text.Json — JSON array Deserialize (non-streaming)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Deserialize_JsonArray()
    {
        // Prepare JSON array payload
        await using var buffer = new MemoryStream();
        JsonSerializer.Serialize(
            buffer,
            _arrayData,
            MyJsonContext.Default.MyTypeArray);

        buffer.Position = 0;

        var result = JsonSerializer.Deserialize(
            buffer,
            MyJsonContext.Default.MyTypeArray);
    }

    // ------------------------------------------------------------
    // 8. Baseline — Convert IAsyncEnumerable to array
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Deserialize_Iterate_ToArray()
    {
        var list = new List<MyType>();
        await foreach (var item in _streamData)
        {
            list.Add(item);
        }
    }

    // ------------------------------------------------------------
    // 9. System.Text.Json — SerializeAsync (JSON array)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Serialize_JsonArray_Async()
    {
        await using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(
            stream,
            _arrayData,
            MyJsonContext.Default.MyTypeArray);
    }

    // ------------------------------------------------------------
    // 10. System.Text.Json — DeserializeAsyncEnumerable (JSON array)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Deserialize_JsonArray_AsyncEnumerable()
    {
        // Prepare JSON array payload
        await using var buffer = new MemoryStream();
        JsonSerializer.Serialize(
            buffer,
            _arrayData,
            MyJsonContext.Default.MyTypeArray);

        buffer.Position = 0;

        await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<MyType>(
            buffer,
            jsonSerializerOptions))
        {
            // consume
        }
    }

}

public class MyType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;        
}

[JsonSerializable(typeof(MyType))]
[JsonSerializable(typeof(MyType[]))]
[JsonSerializable(typeof(IAsyncEnumerable<MyType>))]
public partial class MyJsonContext : JsonSerializerContext { }

public class Program
{
    public static void Main(string[] args) => BenchmarkRunner.Run<StreamingBenchmarks>(args: args);
}