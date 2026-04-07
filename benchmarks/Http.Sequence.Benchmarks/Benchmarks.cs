using System.Text.Json;
using System.Text.Json.Serialization;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using Juner.Sequence;
using Juner.Sequence.Extensions;

using static Juner.Http.Sequence.Benchmarks.Settings;

namespace Juner.Http.Sequence.Benchmarks;

public class Benchmarks
{
    private readonly HttpClient _client;

    public Benchmarks() => _client = new HttpClient(new FakeHttpMessageHandler());

    // ------------------------------------------------------------
    // 1. Juner.Http.Sequence — DeserializeAsyncEnumerable
    // ------------------------------------------------------------
    [Benchmark(Description = "1. NDJSON streaming via Juner.Http.Sequence")]
    [BenchmarkCategory("Juner.Http.Sequence", "arraySend")]
    public async Task Deserialize_NdJson_HttpSequence_FlushPerRecord()
    {
        var response = await _client.GetAsync("http://localhost/ndjson");
        var options = SequenceSerializerOptions.JsonLines;

        await foreach (var _ in response.Content.ReadSequenceEnumerable(
            MyJsonContext.Default.MyType,
            options))
        {
            // consume
        }
    }

    // ------------------------------------------------------------
    // 3. System.Text.Json — DeserializeAsyncEnumerable (JSON array)
    // ------------------------------------------------------------
    [Benchmark(Description = "2. JSON array streaming via STJ.DeserializeAsyncEnumerable")]
    [BenchmarkCategory("System.Text.Json", "arraySend")]
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

public class ChunkedHttpMessageHandler : HttpMessageHandler
{
    private readonly byte[] _payload;
    private readonly int _chunkSize;

    public ChunkedHttpMessageHandler(byte[] payload, int chunkSize = 1024)
    {
        _payload = payload;
        _chunkSize = chunkSize;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stream = new ChunkedStream(_payload, _chunkSize);
        var content = new StreamContent(stream);

        content.Headers.ContentType = new("application/x-ndjson");

        return new HttpResponseMessage
        {
            Content = content
        };
    }

    private sealed class ChunkedStream : Stream
    {
        private readonly byte[] _buffer;
        private readonly int _chunkSize;
        private int _position;

        public ChunkedStream(byte[] buffer, int chunkSize)
        {
            _buffer = buffer;
            _chunkSize = chunkSize;
        }

        public override async Task<int> ReadAsync(
            byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_position >= _buffer.Length)
                return 0;

            var remaining = _buffer.Length - _position;
            var toCopy = Math.Min(_chunkSize, remaining);

            Buffer.BlockCopy(_buffer, _position, buffer, offset, toCopy);
            _position += toCopy;

            // simulate flush / chunk delay
            await Task.Yield();

            return toCopy;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _buffer.Length;
        public override long Position { get => _position; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}

public class ChunkedBenchmarks
{
    private readonly HttpClient _client;

    public ChunkedBenchmarks()
    {
        // NDJSON を chunk streaming で送る
        var ndjson = GenerateNdJsonPayload();
        _client = new HttpClient(new ChunkedHttpMessageHandler(ndjson, chunkSize: 1024));
    }

    [Benchmark(Description = "1. NDJSON streaming (chunked sender)")]
    [BenchmarkCategory("NDJSON", "chunkedsend")]
    public async Task Deserialize_NdJson_Chunked()
    {
        var response = await _client.GetAsync("http://localhost/ndjson");

        await foreach (var _ in response.Content.ReadJsonLinesAsyncEnumerable(
            MyJsonContext.Default.MyType))
        {
            // consume
        }
    }

    private static byte[] GenerateNdJsonPayload()
    {
        using var ms = new MemoryStream();
        SequenceSerializer.SerializeAsync(
            ms,
            Enumerable.Range(0, COUNT)
                .Select(i => new MyType { Id = i, Name = $"Item {i}" })
                .ToAsyncEnumerable(),
            MyJsonContext.Default.MyType,
            SequenceSerializerOptions.JsonLines).GetAwaiter().GetResult();
        return ms.ToArray();
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
            .AddExporter(new JunerMarkdownExporter())
            .WithOptions(ConfigOptions.JoinSummary)
            .WithOptions(ConfigOptions.DisableLogFile);
        BenchmarkRunner.Run(typeof(Benchmarks).Assembly, config, args);
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