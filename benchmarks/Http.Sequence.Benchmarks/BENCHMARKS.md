# Juner.Http.Sequence Benchmarks

**Purpose:** Measure HTTP client-side streaming performance using Juner.Http.Sequence.

This benchmark focuses on how fast an `HttpClient` can consume streaming JSON formats:

- **NDJSON** (newline-delimited JSON)
- **JSON Lines** (RFC 7464 style)
- **JSON Sequence** (`0x1E` framed JSON)

All benchmarks use `FakeHttpMessageHandler` to eliminate network overhead and measure pure client-side parsing performance.

- **Runtime:** .NET 10.0.5 (10.0.5, 10.0.526.15411)
- **OS:** Windows 11 (10.0.26200.8117/25H2/2025Update/HudsonValley2)

## Results

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8117/25H2/2025Update/HudsonValley2)
Intel Core i5-9400 CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 7.0  : .NET 7.0.20 (7.0.20, 7.0.2024.26716), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.25 (8.0.25, 8.0.2526.11203), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v3

LaunchCount=1  

```
| Method                                                    | Job       | Runtime   | Mean     | Error    | StdDev   | Ratio | Gen0      | Gen1     | Gen2     | Allocated | Alloc Ratio |
|---------------------------------------------------------- |---------- |---------- |---------:|---------:|---------:|------:|----------:|---------:|---------:|----------:|------------:|
| &#39;NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 10.0 | .NET 10.0 | 43.74 ms | 0.543 ms | 0.508 ms |  0.64 | 1916.6667 | 250.0000 | 250.0000 |   10.7 MB |        1.00 |
| &#39;NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 7.0  | .NET 7.0  | 68.63 ms | 0.425 ms | 0.397 ms |  1.00 | 1750.0000 | 125.0000 | 125.0000 |   10.7 MB |        1.00 |
| &#39;NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 8.0  | .NET 8.0  | 59.49 ms | 0.797 ms | 0.746 ms |  0.87 | 1750.0000 | 125.0000 | 125.0000 |   10.7 MB |        1.00 |
| &#39;NDJSON streaming via Juner.Http.Sequence&#39;                | .NET 9.0  | .NET 9.0  | 50.13 ms | 0.253 ms | 0.236 ms |  0.73 | 1800.0000 | 200.0000 | 200.0000 |   10.7 MB |        1.00 |
|                                                           |           |           |          |          |          |       |           |          |          |           |             |
| &#39;JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 10.0 | .NET 10.0 | 26.67 ms | 0.149 ms | 0.124 ms |  0.57 | 2031.2500 | 562.5000 | 343.7500 |   10.7 MB |        1.00 |
| &#39;JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 7.0  | .NET 7.0  | 46.66 ms | 0.283 ms | 0.265 ms |  1.00 | 1818.1818 | 272.7273 | 181.8182 |   10.7 MB |        1.00 |
| &#39;JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 8.0  | .NET 8.0  | 36.61 ms | 0.209 ms | 0.186 ms |  0.78 | 1928.5714 | 500.0000 | 285.7143 |   10.7 MB |        1.00 |
| &#39;JSON array streaming via STJ.DeserializeAsyncEnumerable&#39; | .NET 9.0  | .NET 9.0  | 29.72 ms | 0.233 ms | 0.206 ms |  0.64 | 2031.2500 | 593.7500 | 343.7500 |   10.7 MB |        1.00 |

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
