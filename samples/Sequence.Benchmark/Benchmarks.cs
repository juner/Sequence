using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Buffers;
using System.Text.Json;
using Juner.Sequence;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using Juner.Sequence.Extensions;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Toolchains.DotNetCli;
using BenchmarkDotNet.Toolchains.CsProj;

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
}

public class MyType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;        
}

[JsonSerializable(typeof(MyType))]
[JsonSerializable(typeof(MyType[]))]
public partial class MyJsonContext : JsonSerializerContext { }

public class Program
{
    public static void Main(string[] args)
    {
        var config = ManualConfig.CreateEmpty()
            .AddJob(CreateJob("net7.0"))
            .AddJob(CreateJob("net8.0"))
            .AddJob(CreateJob("net9.0"))
            .AddJob(CreateJob("net10.0"))
            .AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default)
            .AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Default)
            .AddColumnProvider(BenchmarkDotNet.Columns.DefaultColumnProviders.Instance);

        BenchmarkRunner.Run<StreamingBenchmarks>(config, args);
    }

    private static Job CreateJob(string tfm)
     => Job.Default
            .WithToolchain(CsProjCoreToolchain.From(
                new NetCoreAppSettings(
                    targetFrameworkMoniker: tfm,
                    runtimeFrameworkVersion: null,
                    name: tfm)))
            .WithId(tfm);
}