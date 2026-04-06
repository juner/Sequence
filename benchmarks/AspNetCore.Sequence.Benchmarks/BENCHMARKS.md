# Juner.AspNetCore.Sequence Benchmarks

- **Dataset:** 100,000 items of `MyType` (Id + Name)
- **Format:** 
   - NDJSON (full streaming)
   - JSON array (buffered)
   - JSON array (IAsyncEnumerable streaming)
   - JSON array (IEnumerable streaming)
- **Purpose:** Compare Juner.AspNetCore.Sequence streaming with STJ's JSON array streaming in a minimal API scenario.

**Runtime:** .NET 10.0.5 (10.0.5, 10.0.526.15411)
**OS:** Windows 11 (10.0.28020.1803)

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
| Method                                                              | Job       | Runtime   | Mean          | Error         | StdDev         | Median        | Ratio | RatioSD | Gen0       | Gen1      | Gen2      | Allocated   | Alloc Ratio |
|-------------------------------------------------------------------- |---------- |---------- |--------------:|--------------:|---------------:|--------------:|------:|--------:|-----------:|----------:|----------:|------------:|------------:|
| &#39;3. JSON array — first-byte latency（buffered）&#39;                      | .NET 10.0 | .NET 10.0 | 117,621.81 μs |  2,342.027 μs |   3,045.297 μs | 116,780.50 μs |  0.86 |    0.07 |  1000.0000 |         - |         - |  9796.13 KB |        1.00 |
| &#39;3. JSON array — first-byte latency（buffered）&#39;                      | .NET 7.0  | .NET 7.0  | 138,303.70 μs |  3,585.845 μs |  10,572.945 μs | 135,348.99 μs |  1.01 |    0.11 |  1500.0000 | 1250.0000 |  500.0000 |  9809.51 KB |        1.00 |
| &#39;3. JSON array — first-byte latency（buffered）&#39;                      | .NET 8.0  | .NET 8.0  | 106,818.46 μs |  3,816.177 μs |  11,252.084 μs | 107,960.05 μs |  0.78 |    0.10 |  1000.0000 |         - |         - |  9795.96 KB |        1.00 |
| &#39;3. JSON array — first-byte latency（buffered）&#39;                      | .NET 9.0  | .NET 9.0  | 103,343.46 μs |  2,036.759 μs |   3,567.220 μs | 102,465.00 μs |  0.75 |    0.06 |  1500.0000 |  750.0000 |  250.0000 |   9804.4 KB |        1.00 |
|                                                                     |           |           |               |               |                |               |       |         |            |           |           |             |             |
| &#39;4. JSON array — full response latency（buffered）&#39;                   | .NET 10.0 | .NET 10.0 | 116,651.87 μs |  2,248.340 μs |   2,843.430 μs | 115,654.57 μs |  0.92 |    0.03 |  3000.0000 | 2000.0000 |  666.6667 | 22928.98 KB |        1.00 |
| &#39;4. JSON array — full response latency（buffered）&#39;                   | .NET 7.0  | .NET 7.0  | 127,420.44 μs |  2,444.739 μs |   3,091.812 μs | 127,011.57 μs |  1.00 |    0.03 |  3250.0000 | 2500.0000 |  750.0000 | 22945.92 KB |        1.00 |
| &#39;4. JSON array — full response latency（buffered）&#39;                   | .NET 8.0  | .NET 8.0  | 120,460.23 μs |  2,279.333 μs |   2,020.570 μs | 119,912.42 μs |  0.95 |    0.03 |  3000.0000 | 2333.3333 |  666.6667 |  22925.7 KB |        1.00 |
| &#39;4. JSON array — full response latency（buffered）&#39;                   | .NET 9.0  | .NET 9.0  | 122,171.63 μs |  2,291.082 μs |   4,882.483 μs | 120,917.70 μs |  0.96 |    0.04 |  3000.0000 | 2000.0000 |  666.6667 |    22938 KB |        1.00 |
|                                                                     |           |           |               |               |                |               |       |         |            |           |           |             |             |
| &#39;5. JSON array — first-byte latency（IAsyncEnumerable streaming）&#39;    | .NET 10.0 | .NET 10.0 |      38.18 μs |      1.774 μs |       5.231 μs |      35.81 μs |     ? |       ? |     3.4180 |         - |         - |    14.28 KB |           ? |
| &#39;5. JSON array — first-byte latency（IAsyncEnumerable streaming）&#39;    | .NET 7.0  | .NET 7.0  |            NA |            NA |             NA |            NA |     ? |       ? |         NA |        NA |        NA |          NA |           ? |
| &#39;5. JSON array — first-byte latency（IAsyncEnumerable streaming）&#39;    | .NET 8.0  | .NET 8.0  |      60.44 μs |      1.206 μs |       3.262 μs |      59.48 μs |     ? |       ? |     4.6387 |    0.4883 |         - |    18.69 KB |           ? |
| &#39;5. JSON array — first-byte latency（IAsyncEnumerable streaming）&#39;    | .NET 9.0  | .NET 9.0  |      41.94 μs |      0.835 μs |       1.440 μs |      41.71 μs |     ? |       ? |     3.4180 |    0.2441 |         - |    14.32 KB |           ? |
|                                                                     |           |           |               |               |                |               |       |         |            |           |           |             |             |
| &#39;6. JSON array — full response latency（IAsyncEnumerable streaming）&#39; | .NET 10.0 | .NET 10.0 | 275,726.64 μs |  5,405.211 μs |   9,030.893 μs | 276,869.55 μs |     ? |       ? |  8000.0000 | 1000.0000 |         - | 57874.12 KB |           ? |
| &#39;6. JSON array — full response latency（IAsyncEnumerable streaming）&#39; | .NET 7.0  | .NET 7.0  |            NA |            NA |             NA |            NA |     ? |       ? |         NA |        NA |        NA |          NA |           ? |
| &#39;6. JSON array — full response latency（IAsyncEnumerable streaming）&#39; | .NET 8.0  | .NET 8.0  | 252,167.68 μs |  3,446.220 μs |   2,877.750 μs | 251,363.90 μs |     ? |       ? |  7000.0000 | 1000.0000 |         - | 52852.65 KB |           ? |
| &#39;6. JSON array — full response latency（IAsyncEnumerable streaming）&#39; | .NET 9.0  | .NET 9.0  | 268,574.82 μs |  5,336.410 μs |  12,261.291 μs | 266,727.30 μs |     ? |       ? |  9000.0000 | 2000.0000 | 1000.0000 | 59201.48 KB |           ? |
|                                                                     |           |           |               |               |                |               |       |         |            |           |           |             |             |
| &#39;7. JSON enumerable — first-byte latency（sync enumeration）&#39;         | .NET 10.0 | .NET 10.0 | 150,021.38 μs | 17,282.076 μs |  50,412.631 μs | 147,922.38 μs |     ? |       ? |  4625.0000 |  125.0000 |         - | 14101.55 KB |           ? |
| &#39;7. JSON enumerable — first-byte latency（sync enumeration）&#39;         | .NET 7.0  | .NET 7.0  |            NA |            NA |             NA |            NA |     ? |       ? |         NA |        NA |        NA |          NA |           ? |
| &#39;7. JSON enumerable — first-byte latency（sync enumeration）&#39;         | .NET 8.0  | .NET 8.0  |   1,168.47 μs |     20.674 μs |      19.339 μs |   1,168.86 μs |     ? |       ? |    42.9688 |         - |         - |   177.29 KB |           ? |
| &#39;7. JSON enumerable — first-byte latency（sync enumeration）&#39;         | .NET 9.0  | .NET 9.0  | 214,631.33 μs | 24,491.836 μs |  69,876.591 μs | 208,106.66 μs |     ? |       ? |  5250.0000 |  125.0000 |         - | 15921.59 KB |           ? |
|                                                                     |           |           |               |               |                |               |       |         |            |           |           |             |             |
| &#39;8. JSON enumerable — full response latency（sync enumeration）&#39;      | .NET 10.0 | .NET 10.0 | 236,561.82 μs |  9,384.550 μs |  27,076.573 μs | 230,788.30 μs |     ? |       ? |  3000.0000 | 1000.0000 |         - | 29489.97 KB |           ? |
| &#39;8. JSON enumerable — full response latency（sync enumeration）&#39;      | .NET 7.0  | .NET 7.0  |            NA |            NA |             NA |            NA |     ? |       ? |         NA |        NA |        NA |          NA |           ? |
| &#39;8. JSON enumerable — full response latency（sync enumeration）&#39;      | .NET 8.0  | .NET 8.0  | 245,489.75 μs | 25,763.934 μs |  75,561.164 μs | 257,069.30 μs |     ? |       ? |  4000.0000 | 1000.0000 |         - | 29490.87 KB |           ? |
| &#39;8. JSON enumerable — full response latency（sync enumeration）&#39;      | .NET 9.0  | .NET 9.0  | 162,447.73 μs |  6,162.944 μs |  17,382.695 μs | 157,808.10 μs |     ? |       ? |  4000.0000 | 2000.0000 |  500.0000 |  29503.7 KB |           ? |
|                                                                     |           |           |               |               |                |               |       |         |            |           |           |             |             |
| &#39;1. NDJSON — first-byte latency&#39;                                    | .NET 10.0 | .NET 10.0 |      43.17 μs |      1.108 μs |       3.267 μs |      42.26 μs |  0.46 |    0.04 |     5.1270 |         - |         - |     21.1 KB |        0.51 |
| &#39;1. NDJSON — first-byte latency&#39;                                    | .NET 7.0  | .NET 7.0  |      94.80 μs |      1.846 μs |       2.126 μs |      94.39 μs |  1.00 |    0.03 |    10.7422 |    2.4414 |         - |    41.23 KB |        1.00 |
| &#39;1. NDJSON — first-byte latency&#39;                                    | .NET 8.0  | .NET 8.0  |      82.58 μs |      1.636 μs |       1.750 μs |      82.38 μs |  0.87 |    0.03 |     8.7891 |    1.7090 |         - |    33.82 KB |        0.82 |
| &#39;1. NDJSON — first-byte latency&#39;                                    | .NET 9.0  | .NET 9.0  |      53.15 μs |      0.955 μs |       0.981 μs |      53.16 μs |  0.56 |    0.02 |     5.3711 |    0.4883 |         - |    21.39 KB |        0.52 |
|                                                                     |           |           |               |               |                |               |       |         |            |           |           |             |             |
| &#39;2. NDJSON — full response latency&#39;                                 | .NET 10.0 | .NET 10.0 | 304,317.25 μs | 15,397.171 μs |  45,398.910 μs | 282,619.35 μs |  0.62 |    0.15 | 18000.0000 |         - |         - | 72710.21 KB |        0.85 |
| &#39;2. NDJSON — full response latency&#39;                                 | .NET 7.0  | .NET 7.0  | 513,060.02 μs | 34,549.871 μs | 101,871.083 μs | 515,476.50 μs |  1.04 |    0.30 | 21000.0000 |         - |         - | 85794.71 KB |        1.00 |
| &#39;2. NDJSON — full response latency&#39;                                 | .NET 8.0  | .NET 8.0  | 449,895.05 μs | 35,090.728 μs | 103,465.811 μs | 463,192.75 μs |  0.91 |    0.28 | 18000.0000 |         - |         - | 72977.15 KB |        0.85 |
| &#39;2. NDJSON — full response latency&#39;                                 | .NET 9.0  | .NET 9.0  | 352,810.68 μs | 17,546.931 μs |  51,462.115 μs | 339,211.00 μs |  0.72 |    0.18 | 17000.0000 |         - |         - | 70839.16 KB |        0.83 |

Benchmarks with issues:
  Benchmarks.'5. JSON array — first-byte latency（IAsyncEnumerable streaming）': .NET 7.0(Runtime=.NET 7.0, Toolchain=net7.0, LaunchCount=1)
  Benchmarks.'6. JSON array — full response latency（IAsyncEnumerable streaming）': .NET 7.0(Runtime=.NET 7.0, Toolchain=net7.0, LaunchCount=1)
  Benchmarks.'7. JSON enumerable — first-byte latency（sync enumeration）': .NET 7.0(Runtime=.NET 7.0, Toolchain=net7.0, LaunchCount=1)
  Benchmarks.'8. JSON enumerable — full response latency（sync enumeration）': .NET 7.0(Runtime=.NET 7.0, Toolchain=net7.0, LaunchCount=1)

### Method definitions

- **NdJson_FirstByte** — Time until the first NDJSON line is received.
- **NdJson_Full** — Time to read the entire NDJSON response.
- **JsonArray_FirstByte** — Time until the first byte of a *buffered* JSON array is received.
- **JsonArray_Full** — Time to read the entire buffered JSON array.
- **JsonStream_FirstByte** — First-byte latency of JSON array *streaming* using `IAsyncEnumerable<T>`.
- **JsonStream_Full** — Full response latency of JSON array streaming using `IAsyncEnumerable<T>`.
- **JsonEnumerable_FirstByte** — First-byte latency when returning `IEnumerable<T>` produced by synchronously consuming an `IAsyncEnumerable<T>`.
- **JsonEnumerable_Full** — Full response latency when returning `IEnumerable<T>` produced by synchronously consuming an `IAsyncEnumerable<T>`.

### Benchmark categories

- **NDJSON** — Fully streaming NDJSON output using Juner.AspNetCore.Sequence.
- **JsonArrayBuffered** — Standard JSON array serialization (fully buffered).
- **JsonArrayStreaming** — JSON array streaming using `IAsyncEnumerable<T>`.
- **JsonEnumerableSync** — Synchronous enumeration of an `IAsyncEnumerable<T>` (non-streaming).

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

Note: You can run any target framework (.NET 10.0, .NET 7.0, .NET 8.0, .NET 9.0).
BenchmarkDotNet will automatically build and execute all configured jobs.

---

## Notes

This benchmark is intended to show **relative performance characteristics**, not absolute throughput numbers.  Different machines will produce different absolute timings,  but the relationships between methods remain consistent.
