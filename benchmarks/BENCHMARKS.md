# Juner.Sequence Benchmarks

This document contains the full BenchmarkDotNet output for Juner.Sequence.

- Machine: Intel Core i5-9400, Windows 11  
- Dataset: 100,000 items of `MyType`  
- Formats: NDJSON (streaming), JSON Array (buffered), baseline iteration  
- Runtimes: .NET 7 / 8 / 9 / 10  
- Tool: BenchmarkDotNet v0.15.8  

---

## Summary (All Runtimes)

```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8116/25H2/2025Update/HudsonValley2)
Intel Core i5-9400 CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]  : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  net10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  net7.0  : .NET 7.0.20 (7.0.20, 7.0.2024.26716), X64 RyuJIT x86-64-v3
  net8.0  : .NET 8.0.25 (8.0.25, 8.0.2526.11203), X64 RyuJIT x86-64-v3
  net9.0  : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v3
```

| Method                     | Job     | Toolchain | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated  | Alloc Ratio |
|--------------------------- |-------- |---------- |---------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|-----------:|------------:|
| Serialize_NdJson_Streaming | net10.0 | net10.0   | 52.50 ms | 0.456 ms | 0.427 ms |  1.29 |    0.16 | 2222.2222 | 1000.0000 | 1000.0000 | 14014290 B |   48,660.73 |
| Serialize_JsonArray        | net10.0 | net10.0   | 11.61 ms | 0.125 ms | 0.117 ms |  0.28 |    0.04 |  765.6250 |  750.0000 |  750.0000 |  7540617 B |   26,182.70 |
| Iterate_IAsyncEnumerable   | net10.0 | net10.0   | 41.34 ms | 1.595 ms | 4.702 ms |  1.01 |    0.17 |         - |         - |         - |      288 B |        1.00 |
|                            |         |           |          |          |          |       |         |           |           |           |            |             |
| Serialize_NdJson_Streaming | net7.0  | net7.0    | 70.24 ms | 1.156 ms | 0.965 ms |  1.40 |    0.03 | 1500.0000 |  875.0000 |  875.0000 | 11603655 B |   36,261.42 |
| Serialize_JsonArray        | net7.0  | net7.0    | 20.42 ms | 0.176 ms | 0.164 ms |  0.41 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |   23,564.13 |
| Iterate_IAsyncEnumerable   | net7.0  | net7.0    | 50.27 ms | 0.724 ms | 0.677 ms |  1.00 |    0.02 |         - |         - |         - |      320 B |        1.00 |
|                            |         |           |          |          |          |       |         |           |           |           |            |             |
| Serialize_NdJson_Streaming | net8.0  | net8.0    | 59.49 ms | 0.307 ms | 0.257 ms |  1.29 |    0.10 | 1555.5556 |  888.8889 |  888.8889 | 11603966 B |   40,291.55 |
| Serialize_JsonArray        | net8.0  | net8.0    | 15.25 ms | 0.109 ms | 0.102 ms |  0.33 |    0.02 |  765.6250 |  750.0000 |  750.0000 |  7540522 B |   26,182.37 |
| Iterate_IAsyncEnumerable   | net8.0  | net8.0    | 46.24 ms | 1.092 ms | 3.221 ms |  1.01 |    0.10 |         - |         - |         - |      288 B |        1.00 |
|                            |         |           |          |          |          |       |         |           |           |           |            |             |
| Serialize_NdJson_Streaming | net9.0  | net9.0    | 58.27 ms | 0.903 ms | 0.844 ms |  1.33 |    0.15 | 1888.8889 |  888.8889 |  888.8889 | 13202950 B |   45,843.58 |
| Serialize_JsonArray        | net9.0  | net9.0    | 13.10 ms | 0.126 ms | 0.105 ms |  0.30 |    0.03 |  765.6250 |  750.0000 |  750.0000 |  7540797 B |   26,183.32 |
| Iterate_IAsyncEnumerable   | net9.0  | net9.0    | 44.47 ms | 1.504 ms | 4.435 ms |  1.01 |    0.15 |         - |         - |         - |      288 B |        1.00 |

```
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
  Gen0        : GC Generation 0 collects per 1000 operations
  Gen1        : GC Generation 1 collects per 1000 operations
  Gen2        : GC Generation 2 collects per 1000 operations
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 ms        : 1 Millisecond (0.001 sec)
```

---

## Interpretation

- NDJSON streaming = enumeration cost + write cost  
- JSON array is fastest due to single-buffer writes  
- All runtimes improve from .NET 7 → 10  
- Memory usage scales with output size (expected for JSON serialization)

---

## Reproduction

Run the benchmark project:

```shell
dotnet run -c Release
```

## Notes

BenchmarkDotNet builds separate executables for each target runtime.  
The benchmark project targets multiple TFMs to enable cross-runtime comparison.