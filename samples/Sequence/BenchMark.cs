#!/usr/bin/env dotnet run
#:sdk Microsoft.NET.Sdk
#:project ../../Sequence/Juner.Sequence.csproj
#:property TargetFramework=net10.0
#:property TargetFrameworks=net8.0;net9.0;net10.0
#:property PublishAot=false
#:property Configuration=Release
#:property Optimize=true

using System.Diagnostics;
using System.IO.Pipelines;
using System.Text.Json.Serialization;
using Juner.Sequence;

// ==========================
// Benchmark Config
// ==========================
const int Count = 100_000;

// ==========================
// Data
// ==========================
static async IAsyncEnumerable<TestData> Generate(int count)
{
    for (var i = 0; i < count; i++)
    {
        yield return new TestData(i, $"Name-{i}");
    }
}

// ==========================
// Benchmark Runner
// ==========================
static async Task RunAsync(string[] args, CancellationToken cancellationToken = default)
{
    var count = Count;
    if (args is { Length: > 0 } && int.Parse(args[0]) is { } count2)
        count = count2;
    var jsonTypeInfo = AppJsonContext.Default.TestData;

    Console.WriteLine($"Count: {count}");
    Console.WriteLine();

    // ==========================
    // Serialize Benchmark
    // ==========================
    {
        var pipe = new Pipe();

        var sw = Stopwatch.StartNew();

        var writerTask = WriteToComplete();
        async Task WriteToComplete()
        {
            await SequenceSerializer.SerializeAsync(
                pipe.Writer,
                Generate(count),
                jsonTypeInfo,
                SequenceSerializerOptions.JsonLines,
                cancellationToken);
            await pipe.Writer.CompleteAsync();
        }

        var readerTask = Consume(pipe.Reader);

        await Task.WhenAll(writerTask, readerTask);

        sw.Stop();

        Console.WriteLine($"Serialize: {sw.ElapsedMilliseconds} ms");
    }

    // ==========================
    // Deserialize Benchmark
    // ==========================
    {
        var pipe = new Pipe();

        // 事前にデータ流し込む
        await SequenceSerializer.SerializeAsync(
            pipe.Writer,
            Generate(count),
            jsonTypeInfo,
            SequenceSerializerOptions.JsonLines,
            cancellationToken);

        await pipe.Writer.CompleteAsync();

        var sw = Stopwatch.StartNew();

        var readCount = 0;

        await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
            pipe.Reader,
            jsonTypeInfo,
            SequenceSerializerOptions.JsonLines,
            cancellationToken))
        {
            readCount++;
        }

        sw.Stop();

        Console.WriteLine($"Deserialize: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Read Count: {readCount}");
    }
}

// ==========================
// Helper
// ==========================
static async Task Consume(PipeReader reader)
{
    while (true)
    {
        var result = await reader.ReadAsync();
        var buffer = result.Buffer;

        // discard
        reader.AdvanceTo(buffer.End);

        if (result.IsCompleted)
            break;
    }

    await reader.CompleteAsync();
}

var source = new CancellationTokenSource();
Console.CancelKeyPress += (o, v) =>
{
    if (v.Cancel) source.CancelAsync();
};
// ==========================
// Run
// ==========================
try
{
    await RunAsync(args, source.Token);
    return 0;
}
catch (OperationCanceledException)
{
    return 1;
}
catch (Exception e)
{
    Console.Error.WriteLine(e.Message);
    return -1;
}


// ==========================
// Model
// ==========================
record TestData(int Id, string Name);

// ==========================
// Source Generator Context
// ==========================
[JsonSerializable(typeof(TestData))]
internal partial class AppJsonContext : JsonSerializerContext
{
}