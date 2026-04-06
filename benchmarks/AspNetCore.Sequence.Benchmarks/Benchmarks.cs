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

using static Juner.AspNetCore.Sequence.Benchmarks.Settings;

namespace Juner.AspNetCore.Sequence.Benchmarks;

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

                        endpoint.MapGet("/ndjson", () => TypedResults.JsonLine(MinimalApiStreamingBenchmarks.GetItems()));

                        endpoint.MapGet("/json-stream", () => TypedResults.Json(MinimalApiStreamingBenchmarks.GetItems(), MyJsonContext.Default.Options));
                        endpoint.MapGet("/json-array", async (CancellationToken cancellationToken)
                            => TypedResults.Json(
#if NET8_0_OR_GREATER
                            await MinimalApiStreamingBenchmarks.GetItems().ToListAsync(cancellationToken),
#else
                            await ToListAsync(MinimalApiStreamingBenchmarks.GetItems(), cancellationToken),
#endif
                            MyJsonContext.Default.Options
                            )
                        );
                        endpoint.MapGet("/json-enumerable-sync", () => TypedResults.Json(MinimalApiStreamingBenchmarks.GetItemsSync()));
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

    /// <summary>
    /// {COUNT} items を返す IAsyncEnumerable を生成する。実際のシナリオでは、これがデータベースクエリや外部 API 呼び出しなどになる可能性がある。
    /// </summary>
    /// <returns></returns>
    private static async IAsyncEnumerable<MyType> GetItems()
    {
        for (var i = 0; i < COUNT; i++)
        {
            yield return new MyType { Id = i, Name = $"Item {i}" };
            await Task.Yield();
        }
    }

    /// <summary>
    /// GetItems() を同期的に列挙するためのヘルパー。これにより、JSON シーケンスが完全に生成される前に最初のバイトが返されるかどうかをテストできる。
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<MyType> GetItemsSync()
    {
        var asyncEnumerater = GetItems().GetAsyncEnumerator();
        do
        {
            var moveNext = asyncEnumerater.MoveNextAsync().AsTask().Result;
            if (!moveNext) yield break;
            if (asyncEnumerater.Current is not null)
                yield return asyncEnumerater.Current;
        } while (true);
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

    // ------------------------------------------------------------
    // 7. JSON enumerable — first-byte latency
    // ------------------------------------------------------------
    [Benchmark]
    public async Task JsonEnumerable_FirstByte()
    {
        var response = await _client.GetAsync("/json-enumerable-sync", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();

        // JSON array は最初の 1 バイトが返るまで遅い
        _ = stream.ReadByte();
    }

    // ------------------------------------------------------------
    // 8. JSON enumerable — full response latency
    // ------------------------------------------------------------
    [Benchmark]
    public async Task JsonEnumerable_Full()
    {
        var response = await _client.GetAsync("/json-enumerable-sync", HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();

        using var reader = new StreamReader(stream);
        _ = await reader.ReadToEndAsync();
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
[JsonSerializable(typeof(IEnumerable<MyType>))]
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
        sw.WriteLine($"""
        # Juner.AspNetCore.Sequence Benchmarks

        - **Dataset:** {COUNT:#,#} items of `MyType` (Id + Name)
        - **Format:** 
           - NDJSON (full streaming)
           - JSON array (buffered)
           - JSON array (IAsyncEnumerable streaming)
           - JSON array (IEnumerable streaming)
        - **Purpose:** Compare Juner.AspNetCore.Sequence streaming with STJ's JSON array streaming in a minimal API scenario.

        **Runtime:** {summary.HostEnvironmentInfo.RuntimeVersion}
        **OS:** {summary.HostEnvironmentInfo.Os}

        ## Results
        """);

        MarkdownExporter.GitHub.ExportToLog(summary, logger);
        sw.WriteLine(logger.GetLog());

        sw.WriteLine($"""
        ### Method definitions

        - **{nameof(MinimalApiStreamingBenchmarks.NdJson_FirstByte)}** — Time until the first NDJSON line is received.
        - **{nameof(MinimalApiStreamingBenchmarks.NdJson_Full)}** — Time to read the entire NDJSON response.
        - **{nameof(MinimalApiStreamingBenchmarks.JsonArray_FirstByte)}** — Time until the first byte of a *buffered* JSON array is received.
        - **{nameof(MinimalApiStreamingBenchmarks.JsonArray_Full)}** — Time to read the entire buffered JSON array.
        - **{nameof(MinimalApiStreamingBenchmarks.JsonStream_FirstByte)}** — First-byte latency of JSON array *streaming* using `IAsyncEnumerable<T>`.
        - **{nameof(MinimalApiStreamingBenchmarks.JsonStream_Full)}** — Full response latency of JSON array streaming using `IAsyncEnumerable<T>`.
        - **{nameof(MinimalApiStreamingBenchmarks.JsonEnumerable_FirstByte)}** — First-byte latency when returning `IEnumerable<T>` produced by synchronously consuming an `IAsyncEnumerable<T>`.
        - **{nameof(MinimalApiStreamingBenchmarks.JsonEnumerable_Full)}** — Full response latency when returning `IEnumerable<T>` produced by synchronously consuming an `IAsyncEnumerable<T>`.

        ### About IEnumerable<T> results

        `JsonEnumerable_*` does **not** represent a JSON array materialized in memory.

        Instead, it represents an `IEnumerable<T>` that is produced by synchronously
        blocking on an underlying `IAsyncEnumerable<T>` (`MoveNextAsync().Result`).

        ASP.NET Core treats synchronous enumeration as **non-streaming**, and therefore
        does not flush until the entire JSON array is written. As a result, the response
        behaves like a fully buffered JSON array, even though the data is not buffered
        in memory as a list.

        ### About .NET 7 results

        .NET 7 does not support JsonSerializer for `IAsyncEnumerable<T>` and `IEnumerable<T>`.  
        Therefore, `JsonStream_*` and `JsonEnumerable_*` benchmarks are reported as `NA`.

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