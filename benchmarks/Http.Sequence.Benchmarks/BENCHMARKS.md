# Juner.Http.Sequence Benchmarks

**Purpose:** Measure HTTP client-side streaming performance using Juner.Http.Sequence.

This benchmark focuses on how fast an `HttpClient` can consume streaming JSON formats:

- **NDJSON** (newline-delimited JSON)
- **JSON Lines** (RFC 7464 style)
- **JSON Sequence** (`0x1E` framed JSON)

All benchmarks use `FakeHttpMessageHandler` to eliminate network overhead and measure pure client-side parsing performance.

- **Runtime:** .NET 10.0.5 (10.0.5, 10.0.526.15411)
- **OS:** Windows 11 (10.0.26200.8116/25H2/2025Update/HudsonValley2)

## Results

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8116/25H2/2025Update/HudsonValley2)
Intel Core i5-9400 CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 7.0  : .NET 7.0.20 (7.0.20, 7.0.2024.26716), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.25 (8.0.25, 8.0.2526.11203), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v3

LaunchCount=1  

```
| Method                          | Job       | Runtime   | Mean     | Error    | StdDev   | Gen0      | Gen1     | Gen2     | Allocated |
|-------------------------------- |---------- |---------- |---------:|---------:|---------:|----------:|---------:|---------:|----------:|
| Deserialize_NdJson_HttpSequence | .NET 10.0 | .NET 10.0 | 44.50 ms | 0.557 ms | 0.521 ms | 1916.6667 | 250.0000 | 250.0000 |   10.7 MB |
| Deserialize_JsonArray_STJ       | .NET 10.0 | .NET 10.0 | 27.03 ms | 0.516 ms | 0.458 ms | 2031.2500 | 531.2500 | 343.7500 |   10.7 MB |
| Deserialize_NdJson_HttpSequence | .NET 7.0  | .NET 7.0  | 69.43 ms | 0.636 ms | 0.564 ms | 1625.0000 |        - |        - |   10.7 MB |
| Deserialize_JsonArray_STJ       | .NET 7.0  | .NET 7.0  | 47.04 ms | 0.480 ms | 0.449 ms | 1818.1818 | 272.7273 | 181.8182 |   10.7 MB |
| Deserialize_NdJson_HttpSequence | .NET 8.0  | .NET 8.0  | 60.04 ms | 0.706 ms | 0.590 ms | 1666.6667 |        - |        - |   10.7 MB |
| Deserialize_JsonArray_STJ       | .NET 8.0  | .NET 8.0  | 37.08 ms | 0.610 ms | 0.570 ms | 1928.5714 | 428.5714 | 285.7143 |   10.7 MB |
| Deserialize_NdJson_HttpSequence | .NET 9.0  | .NET 9.0  | 51.01 ms | 0.539 ms | 0.504 ms | 1800.0000 | 200.0000 | 200.0000 |   10.7 MB |
| Deserialize_JsonArray_STJ       | .NET 9.0  | .NET 9.0  | 30.12 ms | 0.405 ms | 0.359 ms | 2031.2500 | 562.5000 | 343.7500 |   10.7 MB |

## Reproduction

Run the benchmark project:

```bash
dotnet run -f net10.0 -c Release -- --launchCount 3
```

BenchmarkDotNet builds separate executables for each target runtime.  
The benchmark project targets multiple TFMs to enable cross-runtime comparison.

Note: You can run any target framework (net7.0, net8.0, net9.0, net10.0).  
BenchmarkDotNet will automatically build and execute all configured jobs.

---

## Notes

This benchmark is intended to show **relative performance characteristics**,  
not absolute throughput numbers.  
Different machines will produce different absolute timings,  
but the relationships between methods remain consistent.
