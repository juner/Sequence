using System.Collections.Immutable;
using System.IO.Pipelines;
using System.Text.Json;
using System.Text.Json.Serialization;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.ConsoleArguments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using Juner.Sequence.Extensions;

using static Juner.Sequence.Benchmarks.Settings;

namespace Juner.Sequence.Benchmarks;

[CategoriesColumn]
public class BenchmarksSerialize
{
    private readonly MyType[] _arrayData;
    private readonly IAsyncEnumerable<MyType> _streamData;

    public BenchmarksSerialize()
    {
        _arrayData = [.. Enumerable.Range(0, COUNT).Select(i => new MyType { Id = i, Name = $"Item {i}" })];
        _streamData = GetStreamData();
    }

    private async IAsyncEnumerable<MyType> GetStreamData()
    {
        foreach (var item in _arrayData)
        {
            yield return item;
            await Task.Yield();
        }
    }

    // ------------------------------------------------------------
    // 00. Baseline — pure IAsyncEnumerable iteration
    // ------------------------------------------------------------
    [Benchmark(Baseline = true, Description = "00. Baseline — pure IAsyncEnumerable iteration")]
    [BenchmarkCategory("Serialize", "Baseline")]
    public async Task Baseline_Serialize()
    {
        await foreach (var _ in _streamData) { }
    }

    // ------------------------------------------------------------
    // 01. NDJSON Serialize (Stream)
    // ------------------------------------------------------------
    [Benchmark(Description = "01. NDJSON Serialize (Stream)")]
    [BenchmarkCategory("Serialize", "Juner.Sequence", "NDJSON")]
    public async Task Serialize_NdJson_Stream()
    {
        await using var stream = new MemoryStream();
        await SequenceSerializer.SerializeAsync(
            stream, _streamData, MyJsonContext.Default.MyType, SequenceSerializerOptions.JsonLines);
    }

    // ------------------------------------------------------------
    // 02. JSON array Serialize (non-streaming)
    // ------------------------------------------------------------
    [Benchmark(Description = "02. JSON array Serialize (non-streaming)")]
    [BenchmarkCategory("Serialize", "System.Text.Json", "JSONArray")]
    public void Serialize_JsonArray()
    {
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, _arrayData, MyJsonContext.Default.MyTypeArray);
    }

    // ------------------------------------------------------------
    // 03. NDJSON Serialize (PipeWriter)
    // ------------------------------------------------------------
    [Benchmark(Description = "03. NDJSON Serialize (PipeWriter)")]
    [BenchmarkCategory("Serialize", "Juner.Sequence", "NDJSON")]
    public async Task Serialize_NdJson_PipeWriter()
    {
        await using var stream = new MemoryStream();
        var writer = PipeWriter.Create(stream);

        await SequenceSerializer.SerializeAsync(
            writer, _streamData, MyJsonContext.Default.MyType, SequenceSerializerOptions.JsonLines);
    }

    // ------------------------------------------------------------
    // 04. JSON array SerializeAsync (STJ)
    // ------------------------------------------------------------
    [Benchmark(Description = "04. SerializeAsync (JSON array)")]
    [BenchmarkCategory("Serialize", "System.Text.Json", "JSONArray")]
    public async Task Serialize_JsonArray_Async()
    {
        await using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, _arrayData, MyJsonContext.Default.MyTypeArray);
    }
}

[CategoriesColumn]
public class BenchmarksDeserialize
{
    private readonly MyType[] _arrayData;
    private readonly IAsyncEnumerable<MyType> _streamData;

    public BenchmarksDeserialize()
    {
        _arrayData = [.. Enumerable.Range(0, COUNT).Select(i => new MyType { Id = i, Name = $"Item {i}" })];
        _streamData = GetStreamData();
    }

    private async IAsyncEnumerable<MyType> GetStreamData()
    {
        foreach (var item in _arrayData)
        {
            yield return item;
            await Task.Yield();
        }
    }

    // ------------------------------------------------------------
    // 00. Baseline — IAsyncEnumerable iteration → array
    // ------------------------------------------------------------
    [Benchmark(Baseline = true, Description = "00. Baseline — Convert IAsyncEnumerable to array")]
    [BenchmarkCategory("Deserialize", "Baseline")]
    public async Task Baseline_Deserialize()
    {
        var list = new List<MyType>();
        await foreach (var item in _streamData) list.Add(item);
    }

    // ------------------------------------------------------------
    // 01. JSON array Deserialize (non-streaming)
    // ------------------------------------------------------------
    [Benchmark(Description = "01. JSON array Deserialize (non-streaming)")]
    [BenchmarkCategory("Deserialize", "System.Text.Json", "JSONArray")]
    public void Deserialize_JsonArray()
    {
        using var buffer = new MemoryStream();
        JsonSerializer.Serialize(buffer, _arrayData, MyJsonContext.Default.MyTypeArray);

        buffer.Position = 0;

        var _ = JsonSerializer.Deserialize(buffer, MyJsonContext.Default.MyTypeArray);
    }
}

[CategoriesColumn]
public class BenchmarksDeserializeAsyncEnumerable
{
    private readonly MyType[] _arrayData;
    private readonly IAsyncEnumerable<MyType> _streamData;

    public BenchmarksDeserializeAsyncEnumerable()
    {
        _arrayData = [.. Enumerable.Range(0, COUNT).Select(i => new MyType { Id = i, Name = $"Item {i}" })];
        _streamData = GetStreamData();
    }

    private async IAsyncEnumerable<MyType> GetStreamData()
    {
        foreach (var item in _arrayData)
        {
            yield return item;
            await Task.Yield();
        }
    }

    // ------------------------------------------------------------
    // 00. Baseline — JSON array Deserialize (non-streaming)
    // ------------------------------------------------------------
    [Benchmark(Baseline = true, Description = "00. Baseline — JSON array Deserialize (non-streaming)")]
    [BenchmarkCategory("DeserializeAsyncEnumerable", "Baseline", "JSONArray")]
    public void Baseline_JsonArray_Deserialize()
    {
        using var buffer = new MemoryStream();
        JsonSerializer.Serialize(buffer, _arrayData, MyJsonContext.Default.MyTypeArray);

        buffer.Position = 0;

        var _ = JsonSerializer.Deserialize(buffer, MyJsonContext.Default.MyTypeArray);
    }


    // ------------------------------------------------------------
    // 01. NDJSON Deserialize (Stream)
    // ------------------------------------------------------------
    [Benchmark(Description = "01. NDJSON Deserialize (Stream)")]
    [BenchmarkCategory("DeserializeAsyncEnumerable", "Juner.Sequence", "NDJSON")]
    public async Task Deserialize_NdJson_Stream()
    {
        await using var buffer = new MemoryStream();
        await SequenceSerializer.SerializeAsync(
            buffer, _streamData, MyJsonContext.Default.MyType, SequenceSerializerOptions.JsonLines);

        buffer.Position = 0;

        await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
            buffer, MyJsonContext.Default.MyType, SequenceSerializerOptions.JsonLines)) { }
    }

    // ------------------------------------------------------------
    // 02. NDJSON Deserialize (PipeReader)
    // ------------------------------------------------------------
    [Benchmark(Description = "02. NDJSON Deserialize (PipeReader)")]
    [BenchmarkCategory("DeserializeAsyncEnumerable", "Juner.Sequence", "NDJSON")]
    public async Task Deserialize_NdJson_PipeReader()
    {
        await using var buffer = new MemoryStream();
        await SequenceSerializer.SerializeAsync(
            buffer, _streamData, MyJsonContext.Default.MyType, SequenceSerializerOptions.JsonLines);

        buffer.Position = 0;

        var reader = PipeReader.Create(buffer);

        await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
            reader, MyJsonContext.Default.MyType, SequenceSerializerOptions.JsonLines)) { }

        await reader.CompleteAsync();
    }

    // ------------------------------------------------------------
    // 03. JSON array DeserializeAsyncEnumerable (STJ)
    // ------------------------------------------------------------
    [Benchmark(Description = "03. DeserializeAsyncEnumerable (JSON array)")]
    [BenchmarkCategory("DeserializeAsyncEnumerable", "System.Text.Json", "JSONArray")]
    public async Task Deserialize_JsonArray_AsyncEnumerable()
    {
        await using var buffer = new MemoryStream();
        JsonSerializer.Serialize(buffer, _arrayData, MyJsonContext.Default.MyTypeArray);

        buffer.Position = 0;

        await foreach (var _ in JsonSerializer.DeserializeAsyncEnumerable<MyType>(
            buffer, MyJsonContext.Default.Options)) { }
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
            .AddExporter(new JunerMarkdownExporter())
            .WithOptions(ConfigOptions.JoinSummary)
            .WithOptions(ConfigOptions.DisableLogFile);
        BenchmarkRunner.Run(typeof(BenchmarksSerialize).Assembly, config, args);
    }
}

public class JunerMarkdownExporter : IExporter
{
    public string Name => "JunerMarkdownExporter";

    public void ExportToLog(Summary summary, ILogger logger) =>
        logger.WriteLine("Exporting benchmark results...");

    public IEnumerable<string> ExportToFiles(Summary summary, ILogger logger)
    {
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

        - **Dataset:** {COUNT:#,#} items of `MyType`
        - **Formats:** NDJSON / JSON array
        - **Purpose:** Compare Juner.Sequence streaming vs System.Text.Json buffered JSON.

        - **Runtime:** {summary.HostEnvironmentInfo.RuntimeVersion}
        - **OS:** {summary.HostEnvironmentInfo.Os}

        ## Results

        """);

        MarkdownExporter.GitHub.ExportToLog(summary, logger);
        sw.WriteLine(logger.GetLog());

        sw.WriteLine($"""
        ## Reproduction

        ```bash
        dotnet run -f net10.0 -c Release -- -r net7.0 net8.0 net9.0 net10.0 --launchCount 1 --memory
        ```

        BenchmarkDotNet builds separate executables for each target runtime.

        ---

        ## Notes

        This benchmark shows **relative performance**, not absolute throughput.
        """);

        return sw.ToString();
    }
}

file static class Settings
{
    public const int COUNT = 100_000;
}