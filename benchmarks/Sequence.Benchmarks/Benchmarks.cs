using System.IO.Pipelines;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using Juner.Sequence.Extensions;

using static Juner.Sequence.Benchmarks.Settings;

namespace Juner.Sequence.Benchmarks;

public class StreamingBenchmarks
{
    private readonly MyType[] _arrayData;
    private readonly IAsyncEnumerable<MyType> _streamData;

    public StreamingBenchmarks()
    {
        _arrayData = [.. Enumerable.Range(0, COUNT).Select(i => new MyType { Id = i, Name = $"Item {i}" })];

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

    // ------------------------------------------------------------
    // 1. Juner.Sequence — NDJSON streaming
    // ------------------------------------------------------------
    [Benchmark(Description = "01. NDJSON streaming")]
    [BenchmarkCategory("Juner.Sequence", "Serialize", "NDJSON")]
    public async Task Serialize_NdJson_Streaming()
    {
        await using var stream = new MemoryStream();

        await SequenceSerializer.SerializeAsync(
            stream,
            _streamData,
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines);
    }

    // ------------------------------------------------------------
    // 2. System.Text.Json — JSON array (non-streaming)
    // ------------------------------------------------------------
    [Benchmark(Description = "02. JSON array (non-streaming)")]
    [BenchmarkCategory("System.Text.Json", "Serialize", "JSONArray")]
    public void Serialize_JsonArray()
    {
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(
            stream,
            _arrayData,
            MyJsonContext.Default.MyTypeArray);
    }

    // ------------------------------------------------------------
    // 3. Baseline — pure IAsyncEnumerable iteration
    // ------------------------------------------------------------
    [Benchmark(Baseline = true, Description = "03. pure IAsyncEnumerable iteration")]
    [BenchmarkCategory("Baseline")]
    public async Task Iterate_IAsyncEnumerable()
    {
        await foreach (var _ in _streamData)
        {
        }
    }

    // ------------------------------------------------------------
    // 4. Juner.Sequence — NDJSON streaming (PipeWriter)
    // ------------------------------------------------------------
    [Benchmark(Description = "4. NDJSON streaming (PipeWriter)")]
    [BenchmarkCategory("Juner.Sequence", "Serialize", "NDJSON")]
    public async Task Serialize_NdJson_PipeWriter_Streaming()
    {
        await using var stream = new MemoryStream();
        var writer = PipeWriter.Create(stream);

        await SequenceSerializer.SerializeAsync(
            writer,
            _streamData,
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines);
    }

    // ------------------------------------------------------------
    // 5. Juner.Sequence — NDJSON Deserialize (Stream)
    // ------------------------------------------------------------
    [Benchmark(Description = "05. NDJSON Deserialize (Stream)")]
    [BenchmarkCategory("Juner.Sequence","Deserialize", "NDJSON")]
    public async Task Deserialize_NdJson_Streaming()
    {
        // Prepare NDJSON payload
        await using var buffer = new MemoryStream();
        await SequenceSerializer.SerializeAsync(
            buffer,
            _streamData,
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines);

        buffer.Position = 0;

        await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
            buffer,
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines))
        {
            // consume
        }
    }

    // ------------------------------------------------------------
    // 6. Juner.Sequence — NDJSON Deserialize (PipeReader)
    // ------------------------------------------------------------
    [Benchmark(Description = "06. NDJSON Deserialize (PipeReader)")]
    [BenchmarkCategory("Juner.Sequence", "Deserialize", "NDJSON")]
    public async Task Deserialize_NdJson_PipeReader_Streaming()
    {
        // Prepare NDJSON payload
        await using var buffer = new MemoryStream();
        await SequenceSerializer.SerializeAsync(
            buffer,
            _streamData,
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines);

        buffer.Position = 0;

        var reader = PipeReader.Create(buffer);

        await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
            reader,
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines))
        {
            // consume
        }

        await reader.CompleteAsync();
    }

    // ------------------------------------------------------------
    // 7. System.Text.Json — JSON array Deserialize (non-streaming)
    // ------------------------------------------------------------
    [Benchmark(Description = "07. JSON array Deserialize (non-streaming)")]
    [BenchmarkCategory("System.Text.Json", "Deserialize", "JSONArray")]
    public void Deserialize_JsonArray()
    {
        // Prepare JSON array payload
        using var buffer = new MemoryStream();
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
    [Benchmark(Description = "08. Convert IAsyncEnumerable to array")]
    [BenchmarkCategory("Baseline")]
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
    [Benchmark(Description = "09. SerializeAsync (JSON array)")]
    [BenchmarkCategory("System.Text.Json", "Serialize", "JSONArray")]
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
    [Benchmark(Description = "10. DeserializeAsyncEnumerable (JSON array)")]
    [BenchmarkCategory("System.Text.Json", "Deserialize", "JSONArray")]
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
            MyJsonContext.Default.Options))
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
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddExporter(new JunerMarkdownExporter());

        BenchmarkRunner.Run<StreamingBenchmarks>(config, args: args);
    }
}

public class JunerMarkdownExporter : IExporter
{
    public string Name => "JunerMarkdownExporter";

    public void ExportToLog(Summary summary, ILogger logger) => logger.WriteLine("Exporting benchmark results...");

    public IEnumerable<string> ExportToFiles(Summary summary, ILogger logger)
    {
        // 出力先が指定されていない場合はデフォルトにする
        var path = Path.Combine(summary.ResultsDirectoryPath, "BENCHMARKS.md");

        var markdown = GenerateMarkdown(summary);

        File.WriteAllText(path, markdown);

        logger.WriteLine($"Benchmark results written to: {path}");

        return [path];
    }

    static string GenerateMarkdown(Summary summary)
    {
        using var sw = new StringWriter();
        var logger = new AccumulationLogger();

        sw.WriteLine($"""
        # Juner.Sequence Benchmarks

        - **Dataset:** {COUNT:#,#} items of `MyType` (Id + Name)
        - **Format:** NDJSON (streaming) vs JSON array (buffered)
        - **Purpose:** Compare Juner.Sequence NDJSON streaming with System.Text.Json JSON array serialization/deserialization.

        - **Runtime:** {summary.HostEnvironmentInfo.RuntimeVersion}
        - **OS:** {summary.HostEnvironmentInfo.Os}

        ## Results

        """);

        MarkdownExporter.GitHub.ExportToLog(summary, logger);
        sw.WriteLine(logger.GetLog());

        sw.WriteLine($"""
        ## Reproduction

        Run the benchmark project:
        ```bash
        dotnet run -f net10.0 -c Release -- -r net7.0 net8.0 net9.0 net10.0 --launchCount 1 --memory
        ```

        BenchmarkDotNet builds separate executables for each target runtime.
        The benchmark project targets multiple TFMs to enable cross-runtime comparison.

        Note: You can run any target framework ({string.Join(", ", summary.Reports.Select(v => v.BenchmarkCase.Job.Environment.Runtime?.Name).OfType<string>().Distinct())}).
        BenchmarkDotNet will automatically build and execute all configured jobs.

        ---

        ## Notes

        This benchmark is intended to show **relative performance characteristics**, not absolute throughput numbers.  Different machines will produce different absolute timings,  but the relationships between methods remain consistent.
        """);
        return sw.ToString();
    }
}

file static class Settings
{
    public const int COUNT = 100_000;
}