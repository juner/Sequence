#!/usr/bin/env dotnet run
#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property TargetFrameworks=net8.0;net9.0;net10.0
#:property PublishAot=false
#:property Configuration=Release
#:property Optimize=true
#:property LangVersion=14
#:package BenchmarkDotNet
#:package Juner.Sequence@1.0.0

using System.IO.Pipelines;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// ==========================
// Entry Point
// ==========================
BenchmarkRunner.Run<Juner.Sequence.BenchMarkSample.SequenceBenchmarks>(args: args);


namespace Juner.Sequence.BenchMarkSample
{
    // ==========================
    // Benchmark Config
    // ==========================
    [InProcess]
    public class SequenceBenchmarks
    {
        [Params(100_000)]
        public int Count;

        private JsonTypeInfo<TestData> _jsonTypeInfo = null!;

        [GlobalSetup]
        public void Setup() => _jsonTypeInfo = AppJsonContext.Default.TestData;

        // ==========================
        // Data
        // ==========================
        private static async IAsyncEnumerable<TestData> Generate(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new TestData(i, $"Name-{i}");
            }
        }

        // ==========================
        // Serialize Benchmark
        // ==========================
        [Benchmark]
        public async Task Serialize()
        {
            var pipe = new Pipe();

            var writerTask = WriteAsync(pipe.Writer, Count);

            async Task WriteAsync(PipeWriter writer, int count, CancellationToken cancellationToken = default)
            {
                await SequenceSerializer.SerializeAsync(
                    writer,
                    Generate(count),
                    _jsonTypeInfo,
                    SequenceSerializerOptions.JsonLines,
                    cancellationToken);
                await writer.CompleteAsync();
            }

            var readerTask = Consume(pipe.Reader);

            await Task.WhenAll(writerTask, readerTask);
        }

        // ==========================
        // Deserialize Benchmark
        // ==========================
        [Benchmark]
        public async Task Deserialize()
        {
            var pipe = new Pipe();
            var WriteTask = Writing(pipe.Writer, _jsonTypeInfo, Count);
            static async Task Writing(PipeWriter writer, JsonTypeInfo<TestData> jsonTypeInfo, int Count, CancellationToken cancellationToken = default)
            {
                // preload
                await SequenceSerializer.SerializeAsync(
                    writer,
                    Generate(Count),
                    jsonTypeInfo,
                    SequenceSerializerOptions.JsonLines,
                    cancellationToken);

                await writer.CompleteAsync();
            }
            var ReadTask = Reading(pipe.Reader, _jsonTypeInfo);
            static async Task<int> Reading(PipeReader reader, JsonTypeInfo<TestData> jsonTypeInfo, CancellationToken cancellationToken = default)
            {
                var readCount = 0;

                await foreach (var _ in SequenceSerializer.DeserializeAsyncEnumerable(
                    reader,
                    jsonTypeInfo,
                    SequenceSerializerOptions.JsonLines,
                    cancellationToken))
                {
                    readCount++;
                }
                return readCount;
            }
            await Task.WhenAll(WriteTask, ReadTask);

        }

        // ==========================
        // Helper
        // ==========================
        private static async Task Consume(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;

                reader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                    break;
            }

            await reader.CompleteAsync();
        }
    }

    // ==========================
    // Model
    // ==========================
    public record TestData(int Id, string Name);

    // ==========================
    // Source Generator Context
    // ==========================
    [JsonSerializable(typeof(TestData))]
    internal partial class AppJsonContext : JsonSerializerContext
    {
    }
}