using System.Text.Json.Serialization;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using Juner.AspNetCore.Sequence.Http;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Juner.AspNetCore.Sequence.Benchmarks;

[
    SimpleJob(RuntimeMoniker.Net10_0),
    SimpleJob(RuntimeMoniker.Net90),
    SimpleJob(RuntimeMoniker.Net80),
    SimpleJob(RuntimeMoniker.Net70)
]
[MemoryDiagnoser]
public class MinimalApiStreamingBenchmarks
{
    private readonly HttpClient _client;

    public MinimalApiStreamingBenchmarks()
    {
        var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.Configure<JsonOptions>(v => v.SerializerOptions.TypeInfoResolver = MyJsonContext.Default);
            })
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoint =>
                    {

                        endpoint.MapGet("/ndjson", () => TypedResults.JsonLine(GetItems()));

                        endpoint.MapGet("/json-stream", () => TypedResults.Json(GetItems(), MyJsonContext.Default.Options));
                        endpoint.MapGet("/json-array", async (HttpContext ctx, CancellationToken cancellationToken)
                            => TypedResults.Json(
#if NET8_0_OR_GREATER
                            await GetItems().ToListAsync(cancellationToken),
#else
                            await ToListAsync(GetItems(), cancellationToken),
#endif
                            MyJsonContext.Default.Options
                            )
                        );
                    });
                });
            })
            .Start();

        _client = host.GetTestClient();
    }

#if !NET8_0_OR_GREATER
    static ValueTask<List<T>> ToListAsync<T>(IAsyncEnumerable<T> values, CancellationToken cancellationToken)
    {
        var list = new List<T>();
        return new ValueTask<List<T>>(ToListAsyncInternal(values, list, cancellationToken));
        static async Task<List<T>> ToListAsyncInternal(IAsyncEnumerable<T> values, List<T> list, CancellationToken cancellationToken)
        {
            await foreach (var item in values.WithCancellation(cancellationToken))
            {
                list.Add(item);
            }
            return list;
        }
    }
#endif
    private async IAsyncEnumerable<MyType> GetItems()
    {
        for (var i = 0; i < 100_000; i++)
        {
            yield return new MyType { Id = i, Name = $"Item {i}" };
            await Task.Yield();
        }
    }

    // ------------------------------------------------------------
    // 1. NDJSON — first-byte latency
    // ------------------------------------------------------------
    [Benchmark]
    public async Task NdJson_FirstByte()
    {
        var response = await _client.GetAsync("/ndjson", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();

        // 最初の 1 行だけ読む
        using var reader = new StreamReader(stream);
        _ = await reader.ReadLineAsync();
    }

    // ------------------------------------------------------------
    // 2. NDJSON — full response latency
    // ------------------------------------------------------------
    [Benchmark]
    public async Task NdJson_Full()
    {
        var response = await _client.GetAsync("/ndjson", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync() is not null) { }
    }

    // ------------------------------------------------------------
    // 3. JSON array — first-byte latency（ほぼ常に遅い）
    // ------------------------------------------------------------
    [Benchmark]
    public async Task JsonArray_FirstByte()
    {
        var response = await _client.GetAsync("/json-array", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();

        // JSON array は最初の 1 バイトが返るまで遅い
        _ = stream.ReadByte();
    }

    // ------------------------------------------------------------
    // 4. JSON array — full response latency
    // ------------------------------------------------------------
    [Benchmark]
    public async Task JsonArray_Full()
    {
        var response = await _client.GetAsync("/json-array", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();

        using var reader = new StreamReader(stream);
        _ = await reader.ReadToEndAsync();
    }
    // ------------------------------------------------------------
    // 5. JSON array — first-byte latency（ほぼ常に遅い）
    // ------------------------------------------------------------
    [Benchmark]
    public async Task JsonStream_FirstByte()
    {
#if NET8_0_OR_GREATER
        var response = await _client.GetAsync("/json-stream", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();

        // IAsyncEnumerable JSON は逐次書き込みされるため first-byte が速い
        _ = stream.ReadByte();
#else
        throw new NotSupportedException("JSON stream first-byte latency benchmark requires .NET 8.0 or greater.");
#endif
    }

    // ------------------------------------------------------------
    // 6. JSON array — full response latency
    // ------------------------------------------------------------
    [Benchmark]
    public async Task JsonStream_Full()
    {
#if NET8_0_OR_GREATER
        var response = await _client.GetAsync("/json-stream", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();

        using var reader = new StreamReader(stream);
        _ = await reader.ReadToEndAsync();
#else
        throw new NotSupportedException("JSON stream full response latency benchmark requires .NET 8.0 or greater.");
#endif
    }
}

public class MyType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}

[JsonSerializable(typeof(MyType))]
[JsonSerializable(typeof(MyType[]))]
[JsonSerializable(typeof(List<MyType>))]
[JsonSerializable(typeof(IAsyncEnumerable<MyType>))]
public partial class MyJsonContext : JsonSerializerContext { }

public class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddExporter(new JunerMarkdownExporter());
        BenchmarkRunner.Run<MinimalApiStreamingBenchmarks>(config, args: args);
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

        sw.WriteLine("# Juner.AspNetCore.Sequence Benchmarks");
        sw.WriteLine();
        sw.WriteLine("**Dataset:** 100,000 items of `MyType` (Id + Name)");
        sw.WriteLine("**Format:** ");
        sw.WriteLine(" - NDJSON (full streaming)");
        sw.WriteLine(" - JSON array (buffered)");
        sw.WriteLine(" - JSON array (IAsyncEnumerable streaming)");
        sw.WriteLine("**Purpose:** Compare Juner.Http.Sequence streaming with STJ's JSON array streaming.");
        sw.WriteLine();
        sw.WriteLine($"**Runtime:** {summary.HostEnvironmentInfo.RuntimeVersion}");
        sw.WriteLine($"**OS:** {summary.HostEnvironmentInfo.Os}");
        sw.WriteLine();

        sw.WriteLine("## Results");
        sw.WriteLine();

        MarkdownExporter.GitHub.ExportToLog(summary, logger);
        sw.Write(logger.GetLog());
        sw.WriteLine();

        sw.WriteLine("## Reproduction");
        sw.WriteLine();
        sw.WriteLine("Run the benchmark project:");
        sw.WriteLine();
        sw.WriteLine("```bash");
        sw.WriteLine("dotnet run -f net10.0 -c Release -- --launchCount 1");
        sw.WriteLine("```");
        sw.WriteLine();
        sw.WriteLine("BenchmarkDotNet builds separate executables for each target runtime. ");
        sw.WriteLine("The benchmark project targets multiple TFMs to enable cross-runtime comparison.");
        sw.WriteLine();
        sw.WriteLine("Note: You can run any target framework (net7.0, net8.0, net9.0, net10.0).");
        sw.WriteLine("BenchmarkDotNet will automatically build and execute all configured jobs.");
        sw.WriteLine();
        sw.WriteLine("---");
        sw.WriteLine();
        sw.WriteLine("## Notes");
        sw.WriteLine();
        sw.WriteLine("This benchmark is intended to show **relative performance characteristics**, not absolute throughput numbers.  Different machines will produce different absolute timings,  but the relationships between methods remain consistent.");
        return sw.ToString();
    }
}