using System.Text.Json;
using System.Text.Json.Serialization;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using Juner.Sequence;
using Juner.Sequence.Extensions;

using static Juner.Http.Sequence.Benchmarks.Settings;

namespace Juner.Http.Sequence.Benchmarks;

[
    SimpleJob(RuntimeMoniker.Net10_0),
    SimpleJob(RuntimeMoniker.Net90),
    SimpleJob(RuntimeMoniker.Net80),
    SimpleJob(RuntimeMoniker.Net70)
]
[MemoryDiagnoser]
public class HttpSequenceBenchmarks
{
    private readonly HttpClient _client;

    public HttpSequenceBenchmarks() => _client = new HttpClient(new FakeHttpMessageHandler());

    // ------------------------------------------------------------
    // 1. Juner.Http.Sequence — DeserializeAsyncEnumerable
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Deserialize_NdJson_HttpSequence()
    {
        var response = await _client.GetAsync("http://localhost/ndjson");

        await foreach (var _ in response.Content.ReadJsonLinesAsyncEnumerable(
            MyJsonContext.Default.MyType))
        {
            // consume
        }
    }

    // ------------------------------------------------------------
    // 2. System.Text.Json — DeserializeAsyncEnumerable (JSON array)
    // ------------------------------------------------------------
    [Benchmark]
    public async Task Deserialize_JsonArray_STJ()
    {
        var response = await _client.GetAsync("http://localhost/json-array");

        await foreach (var _ in JsonSerializer.DeserializeAsyncEnumerable(
            await response.Content.ReadAsStreamAsync(),
            MyJsonContext.Default.MyType))
        {
            // consume
        }
    }
}

// ------------------------------------------------------------
// Fake HttpMessageHandler — NDJSON / JSON array を返す
// ------------------------------------------------------------
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly byte[] _ndjson;
    private readonly byte[] _jsonArray;

    public FakeHttpMessageHandler()
    {
        // NDJSON
        using var nd = new MemoryStream();
        SequenceSerializer.SerializeAsync(
            nd,
            Enumerable.Range(0, COUNT)
                .Select(i => new MyType { Id = i, Name = $"Item {i}" })
                .ToAsyncEnumerable(),
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines).GetAwaiter().GetResult();
        _ndjson = nd.ToArray();

        // JSON array
        _jsonArray = JsonSerializer.SerializeToUtf8Bytes(
            Enumerable.Range(0, COUNT)
                .Select(i => new MyType { Id = i, Name = $"Item {i}" })
                .ToArray(),
            MyJsonContext.Default.MyTypeArray);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri!.AbsolutePath.Contains("ndjson"))
        {
            return Task.FromResult(new HttpResponseMessage
            {
                Content = new ByteArrayContent(_ndjson)
                {
                    Headers = { ContentType = new("application/x-ndjson") }
                }
            });
        }

        return Task.FromResult(new HttpResponseMessage
        {
            Content = new ByteArrayContent(_jsonArray)
            {
                Headers = { ContentType = new("application/json") }
            }
        });
    }
}

file static class EnumerableExtensions
{
#if !NET8_0_OR_GREATER
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            yield return value;
            await Task.Yield();
        }
    }
#endif
}

// ------------------------------------------------------------
// Model + JsonContext
// ------------------------------------------------------------
public class MyType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}

[JsonSerializable(typeof(MyType))]
[JsonSerializable(typeof(MyType[]))]
public partial class MyJsonContext : JsonSerializerContext
{
}

public class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddExporter(new JunerMarkdownExporter());

        BenchmarkRunner.Run<HttpSequenceBenchmarks>(config, args);
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
        # Juner.Http.Sequence Benchmarks

        **Purpose:** Measure HTTP client-side streaming performance using Juner.Http.Sequence.

        This benchmark focuses on how fast an `HttpClient` can consume streaming JSON formats:

        - **NDJSON** (newline-delimited JSON)
        - **JSON Lines** (RFC 7464 style)
        - **JSON Sequence** (`0x1E` framed JSON)

        All benchmarks use `FakeHttpMessageHandler` to eliminate network overhead and measure pure client-side parsing performance.

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
        dotnet run -f net10.0 -c Release -- --launchCount 1
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