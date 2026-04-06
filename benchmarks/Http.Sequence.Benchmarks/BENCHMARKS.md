# Juner.Http.Sequence Benchmarks

**Purpose:** Measure HTTP client-side streaming performance using Juner.Http.Sequence.

This benchmark focuses on how fast an `HttpClient` can consume streaming JSON formats:

- **NDJSON** (newline-delimited JSON)
- **JSON Lines** (RFC 7464 style)
- **JSON Sequence** (`0x1E` framed JSON)

All benchmarks use `FakeHttpMessageHandler` to eliminate network overhead and measure pure client-side parsing performance.

- **Runtime:** .NET 10.0.5 (10.0.5, 10.0.526.15411)
- **OS:** Windows 11 (10.0.28020.1803)

## Results

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.28020.1803)
Intel Core i7-1065G7 CPU 1.30GHz (Max: 1.50GHz), 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4
  .NET 7.0  : .NET 7.0.20 (7.0.20, 7.0.2024.26716), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.13 (9.0.13, 9.0.1326.6317), X64 RyuJIT x86-64-v4

LaunchCount=1  

```
| Type                  | Method                                                       | Job       | Runtime   | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1     | Gen2     | Allocated | Alloc Ratio |
|---------------------- |------------------------------------------------------------- |---------- |---------- |---------:|---------:|---------:|------:|--------:|----------:|---------:|---------:|----------:|------------:|
| DeserializeBenchmarks | &#39;1. NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 10.0 | .NET 10.0 | 42.04 ms | 0.419 ms | 0.327 ms |  0.58 |    0.01 | 1818.1818 |        - |        - |   10.7 MB |        1.00 |
| DeserializeBenchmarks | &#39;1. NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 7.0  | .NET 7.0  | 71.97 ms | 1.353 ms | 1.266 ms |  1.00 |    0.02 | 1857.1429 |        - |        - |   10.7 MB |        1.00 |
| DeserializeBenchmarks | &#39;1. NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 8.0  | .NET 8.0  | 61.40 ms | 0.556 ms | 0.493 ms |  0.85 |    0.02 | 1875.0000 |        - |        - |   10.7 MB |        1.00 |
| DeserializeBenchmarks | &#39;1. NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 9.0  | .NET 9.0  | 51.93 ms | 0.616 ms | 0.481 ms |  0.72 |    0.01 | 1800.0000 |        - |        - |   10.7 MB |        1.00 |
|                       |                                                              |           |           |          |          |          |       |         |           |          |          |           |             |
| ChunkedBenchmarks     | &#39;1. NDJSON streaming (chunked sender)&#39;                       | .NET 10.0 | .NET 10.0 | 45.69 ms | 0.787 ms | 0.736 ms |  0.60 |    0.01 | 1909.0909 |        - |        - |  11.07 MB |        0.69 |
| ChunkedBenchmarks     | &#39;1. NDJSON streaming (chunked sender)&#39;                       | .NET 7.0  | .NET 7.0  | 76.75 ms | 1.119 ms | 0.992 ms |  1.00 |    0.02 | 2857.1429 | 857.1429 | 857.1429 |  15.99 MB |        1.00 |
| ChunkedBenchmarks     | &#39;1. NDJSON streaming (chunked sender)&#39;                       | .NET 8.0  | .NET 8.0  | 67.63 ms | 1.203 ms | 1.943 ms |  0.88 |    0.03 | 2666.6667 | 666.6667 | 666.6667 |  15.94 MB |        1.00 |
| ChunkedBenchmarks     | &#39;1. NDJSON streaming (chunked sender)&#39;                       | .NET 9.0  | .NET 9.0  | 57.04 ms | 1.106 ms | 1.358 ms |  0.74 |    0.02 | 2666.6667 | 666.6667 | 666.6667 |  15.95 MB |        1.00 |
|                       |                                                              |           |           |          |          |          |       |         |           |          |          |           |             |
| DeserializeBenchmarks | &#39;2. JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 10.0 | .NET 10.0 | 23.50 ms | 0.230 ms | 0.215 ms |  0.50 |    0.01 | 2000.0000 | 156.2500 | 125.0000 |   10.7 MB |        1.00 |
| DeserializeBenchmarks | &#39;2. JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 7.0  | .NET 7.0  | 46.85 ms | 0.592 ms | 0.525 ms |  1.00 |    0.02 | 1818.1818 |        - |        - |   10.7 MB |        1.00 |
| DeserializeBenchmarks | &#39;2. JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 8.0  | .NET 8.0  | 38.42 ms | 0.625 ms | 0.522 ms |  0.82 |    0.01 | 1846.1538 |  76.9231 |        - |   10.7 MB |        1.00 |
| DeserializeBenchmarks | &#39;2. JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 9.0  | .NET 9.0  | 29.05 ms | 0.207 ms | 0.173 ms |  0.62 |    0.01 | 2000.0000 | 156.2500 | 125.0000 |   10.7 MB |        1.00 |

## Reproduction

Run the benchmark project:

```bash
dotnet run -f net10.0 -c Release -- -r net7.0 net8.0 net9.0 net10.0 --launchCount 1 --memory
```

BenchmarkDotNet builds separate executables for each target runtime.
The benchmark project targets multiple TFMs to enable cross-runtime comparison.

Note: You can run any target framework (.NET 10.0, .NET 7.0, .NET 8.0, .NET 9.0).
BenchmarkDotNet will automatically build and execute all configured jobs.

---

## Notes

This benchmark is intended to show **relative performance characteristics**, not absolute throughput numbers.  Different machines will produce different absolute timings,  but the relationships between methods remain consistent.
