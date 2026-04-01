# Juner.AspNetCore.Sequence Benchmarks

**Dataset:** 100,000 items of `MyType` (Id + Name)
**Format:** 
 - NDJSON (full streaming)
 - JSON array (buffered)
 - JSON array (IAsyncEnumerable streaming)
**Purpose:** Compare Juner.Http.Sequence streaming with STJ's JSON array streaming.

**Runtime:** .NET 10.0.5 (10.0.5, 10.0.526.15411)
**OS:** Windows 11 (10.0.28020.1743)

## Results

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.28020.1743)
Intel Core i7-1065G7 CPU 1.30GHz (Max: 1.50GHz), 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4
  .NET 7.0  : .NET 7.0.20 (7.0.20, 7.0.2024.26716), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.13 (9.0.13, 9.0.1326.6317), X64 RyuJIT x86-64-v4

LaunchCount=2  

```
| Method               | Job       | Runtime   | Mean          | Error         | StdDev         | Median        | Gen0       | Gen1      | Gen2      | Allocated   |
|--------------------- |---------- |---------- |--------------:|--------------:|---------------:|--------------:|-----------:|----------:|----------:|------------:|
| NdJson_FirstByte     | .NET 10.0 | .NET 10.0 |      66.60 μs |      2.102 μs |       7.147 μs |      66.59 μs |     5.3711 |         - |         - |    21.28 KB |
| NdJson_Full          | .NET 10.0 | .NET 10.0 | 338,022.33 μs | 14,695.225 μs |  62,220.503 μs | 320,678.95 μs | 17000.0000 |         - |         - | 69462.32 KB |
| JsonArray_FirstByte  | .NET 10.0 | .NET 10.0 | 109,173.02 μs |  1,711.798 μs |   4,479.478 μs | 110,123.92 μs |  1000.0000 |  333.3333 |         - |  9795.88 KB |
| JsonArray_Full       | .NET 10.0 | .NET 10.0 | 130,518.60 μs |  2,727.807 μs |  11,519.922 μs | 128,550.80 μs |  2000.0000 | 1000.0000 |         - | 22914.83 KB |
| JsonStream_FirstByte | .NET 10.0 | .NET 10.0 |      51.32 μs |      1.105 μs |       4.678 μs |      52.42 μs |     3.4180 |    0.2441 |         - |    14.24 KB |
| JsonStream_Full      | .NET 10.0 | .NET 10.0 | 284,865.91 μs |  5,286.466 μs |  22,383.230 μs | 282,852.55 μs |  8000.0000 | 2000.0000 | 1000.0000 | 56890.58 KB |
| NdJson_FirstByte     | .NET 7.0  | .NET 7.0  |     116.08 μs |      1.734 μs |       3.342 μs |     115.30 μs |    10.7422 |    1.9531 |         - |    41.41 KB |
| NdJson_Full          | .NET 7.0  | .NET 7.0  | 470,517.49 μs | 34,741.155 μs | 109,276.891 μs | 411,206.00 μs | 21000.0000 |         - |         - | 85796.77 KB |
| JsonArray_FirstByte  | .NET 7.0  | .NET 7.0  | 112,164.34 μs |  1,945.106 μs |   7,755.021 μs | 109,894.41 μs |  1400.0000 | 1000.0000 |  400.0000 |  9804.13 KB |
| JsonArray_Full       | .NET 7.0  | .NET 7.0  | 140,828.76 μs |  4,472.759 μs |  15,445.681 μs | 134,977.58 μs |  3000.0000 | 2500.0000 |  750.0000 | 22936.06 KB |
| JsonStream_FirstByte | .NET 7.0  | .NET 7.0  |            NA |            NA |             NA |            NA |         NA |        NA |        NA |          NA |
| JsonStream_Full      | .NET 7.0  | .NET 7.0  |            NA |            NA |             NA |            NA |         NA |        NA |        NA |          NA |
| NdJson_FirstByte     | .NET 8.0  | .NET 8.0  |      94.20 μs |      1.361 μs |       2.988 μs |      94.24 μs |     8.7891 |         - |         - |    33.98 KB |
| NdJson_Full          | .NET 8.0  | .NET 8.0  | 356,772.41 μs | 11,276.080 μs |  34,631.173 μs | 341,422.25 μs | 18000.0000 |         - |         - |  73964.4 KB |
| JsonArray_FirstByte  | .NET 8.0  | .NET 8.0  | 100,156.36 μs |  1,729.852 μs |   6,526.276 μs |  98,639.50 μs |  1333.3333 | 1000.0000 |  333.3333 |  9806.56 KB |
| JsonArray_Full       | .NET 8.0  | .NET 8.0  | 157,090.68 μs |  7,788.247 μs |  32,720.138 μs | 139,464.13 μs |  3000.0000 | 2333.3333 |  666.6667 | 22930.86 KB |
| JsonStream_FirstByte | .NET 8.0  | .NET 8.0  |      78.61 μs |      1.189 μs |       4.995 μs |      78.40 μs |     4.3945 |    0.4883 |         - |    18.72 KB |
| JsonStream_Full      | .NET 8.0  | .NET 8.0  | 325,753.44 μs |  4,314.848 μs |   8,814.092 μs | 327,665.20 μs |  7000.0000 | 1000.0000 |         - | 52599.51 KB |
| NdJson_FirstByte     | .NET 9.0  | .NET 9.0  |      71.11 μs |      1.544 μs |       6.505 μs |      70.95 μs |     5.1270 |    0.4883 |         - |    21.01 KB |
| NdJson_Full          | .NET 9.0  | .NET 9.0  | 499,779.32 μs |  7,445.898 μs |  26,975.827 μs | 499,828.95 μs | 17000.0000 |         - |         - | 71262.73 KB |
| JsonArray_FirstByte  | .NET 9.0  | .NET 9.0  | 122,296.97 μs |  1,709.569 μs |   3,606.061 μs | 122,315.43 μs |  1000.0000 |  333.3333 |         - |  9795.95 KB |
| JsonArray_Full       | .NET 9.0  | .NET 9.0  | 133,768.93 μs |  2,166.453 μs |   7,185.906 μs | 133,552.15 μs |  2000.0000 | 1000.0000 |         - |  22914.6 KB |
| JsonStream_FirstByte | .NET 9.0  | .NET 9.0  |      77.84 μs |      1.219 μs |       4.931 μs |      77.80 μs |     3.4180 |         - |         - |    14.42 KB |
| JsonStream_Full      | .NET 9.0  | .NET 9.0  | 333,219.37 μs |  5,437.878 μs |   8,139.160 μs | 333,746.45 μs |  8000.0000 | 1000.0000 |         - | 58031.58 KB |

Benchmarks with issues:
  MinimalApiStreamingBenchmarks.JsonStream_FirstByte: .NET 7.0(Runtime=.NET 7.0, LaunchCount=2)
  MinimalApiStreamingBenchmarks.JsonStream_Full: .NET 7.0(Runtime=.NET 7.0, LaunchCount=2)

## Reproduction

Run the benchmark project:

```bash
dotnet run -f net10.0 -c Release -- --launchCount 1
```

BenchmarkDotNet builds separate executables for each target runtime. 
The benchmark project targets multiple TFMs to enable cross-runtime comparison.

Note: You can run any target framework (net7.0, net8.0, net9.0, net10.0).
BenchmarkDotNet will automatically build and execute all configured jobs.

---

## Notes

This benchmark is intended to show **relative performance characteristics**, not absolute throughput numbers.  Different machines will produce different absolute timings,  but the relationships between methods remain consistent.
