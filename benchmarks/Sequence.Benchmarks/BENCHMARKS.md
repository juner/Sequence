# Juner.Sequence Benchmarks

This document contains the full BenchmarkDotNet output for Juner.Sequence.

- Machine: **Intel Core i7‑1065G7 (Surface Book 3)**  
- Dataset: **100,000 items of `MyType`**  
- Formats: **NDJSON (streaming)**, **JSON Array (buffered)**, **baseline iteration**  
- Runtimes: **.NET 7 / 8 / 9 / 10**  
- Tool: **BenchmarkDotNet v0.15.8**  

---

## Summary (All Runtimes)

```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.28020.1743)
Intel Core i7-1065G7 CPU 1.30GHz (Max: 1.50GHz), 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4
  Job-EWEUQJ : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v4
  Job-QSKNUG : .NET 7.0.20 (7.0.20, 7.0.2024.26716), X64 RyuJIT x86-64-v3
  Job-LZJQCX : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  Job-LEIXTY : .NET 9.0.13 (9.0.13, 9.0.1326.6317), X64 RyuJIT x86-64-v4

LaunchCount = 10
```

---

## Full Results Table

> The following table is the raw BenchmarkDotNet output.  
> No values have been modified.

| Method                                  | Runtime   | Mean      | Error    | StdDev    | Median    | P95       | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated  | Alloc Ratio |
|---------------------------------------- |---------- |----------:|---------:|----------:|----------:|----------:|------:|--------:|----------:|----------:|----------:|-----------:|------------:|
| Serialize_NdJson_Streaming              | .NET 10.0 | 102.32 ms | 2.481 ms | 18.173 ms | 105.21 ms | 136.07 ms |  2.46 |    0.44 | 2000.0000 | 1000.0000 | 1000.0000 | 14043200 B |   48,761.11 |
| Serialize_JsonArray                     | .NET 10.0 |  13.23 ms | 0.318 ms |  2.401 ms |  12.56 ms |  18.13 ms |  0.32 |    0.06 |  734.3750 |  718.7500 |  718.7500 |  7540582 B |   26,182.58 |
| Iterate_IAsyncEnumerable                | .NET 10.0 |  41.71 ms | 0.334 ms |  1.526 ms |  41.39 ms |  44.58 ms |  1.00 |    0.05 |         - |         - |         - |      288 B |        1.00 |
| Serialize_NdJson_PipeWriter_Streaming   | .NET 10.0 | 152.29 ms | 1.188 ms |  4.924 ms | 152.77 ms | 159.62 ms |  3.66 |    0.18 |         - |         - |         - |  8389656 B |   29,130.75 |
| Deserialize_NdJson_Streaming            | .NET 10.0 | 197.32 ms | 3.886 ms | 32.882 ms | 193.47 ms | 258.26 ms |  4.74 |    0.81 | 4000.0000 | 1000.0000 | 1000.0000 | 21974792 B |   76,301.36 |
| Deserialize_NdJson_PipeReader_Streaming | .NET 10.0 | 193.92 ms | 3.673 ms | 32.563 ms | 189.55 ms | 253.16 ms |  4.65 |    0.80 | 4000.0000 | 1000.0000 | 1000.0000 | 21981368 B |   76,324.19 |
| Deserialize_JsonArray                   | .NET 10.0 |  64.13 ms | 1.835 ms | 17.282 ms |  58.11 ms | 102.53 ms |  1.54 |    0.42 | 2000.0000 | 1500.0000 | 1000.0000 | 18362384 B |   63,758.28 |
| Deserialize_Iterate_ToArray             | .NET 10.0 |  44.42 ms | 0.443 ms |  1.995 ms |  44.19 ms |  48.00 ms |  1.07 |    0.06 |  181.8182 |  181.8182 |  181.8182 |  2097941 B |    7,284.52 |
| Serialize_JsonArray_Async               | .NET 10.0 |  13.82 ms | 0.272 ms |  2.396 ms |  13.15 ms |  19.15 ms |  0.33 |    0.06 |  718.7500 |  703.1250 |  703.1250 |  7540757 B |   26,183.18 |
| Deserialize_JsonArray_AsyncEnumerable   | .NET 10.0 |  38.39 ms | 0.343 ms |  2.350 ms |  37.64 ms |  43.17 ms |  0.92 |    0.07 | 2750.0000 | 1000.0000 | 1000.0000 | 15481870 B |   53,756.49 |
|                                         |           |           |          |           |           |           |       |         |           |           |           |            |             |
| Serialize_NdJson_Streaming              | .NET 7.0  |  90.13 ms | 3.317 ms | 21.385 ms |  83.54 ms | 160.12 ms |  1.20 |    0.29 | 1571.4286 |  857.1429 |  857.1429 | 11607039 B |   36,272.00 |
| Serialize_JsonArray                     | .NET 7.0  |  18.78 ms | 0.153 ms |  0.604 ms |  18.65 ms |  19.78 ms |  0.25 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |   23,564.13 |
| Iterate_IAsyncEnumerable                | .NET 7.0  |  75.03 ms | 0.341 ms |  1.227 ms |  75.06 ms |  77.00 ms |  1.00 |    0.02 |         - |         - |         - |      320 B |        1.00 |
| Serialize_NdJson_PipeWriter_Streaming   | .NET 7.0  |  92.84 ms | 0.811 ms |  6.830 ms |  91.16 ms | 105.66 ms |  1.24 |    0.09 | 1500.0000 |  833.3333 |  833.3333 | 11602279 B |   36,257.12 |
| Deserialize_NdJson_Streaming            | .NET 7.0  | 155.36 ms | 1.779 ms | 11.482 ms | 150.72 ms | 179.07 ms |  2.07 |    0.16 | 3500.0000 | 1000.0000 | 1000.0000 | 19538120 B |   61,056.62 |
| Deserialize_NdJson_PipeReader_Streaming | .NET 7.0  | 149.29 ms | 1.187 ms |  7.118 ms | 147.88 ms | 164.28 ms |  1.99 |    0.10 | 3000.0000 | 1000.0000 | 1000.0000 | 19534872 B |   61,046.47 |
| Deserialize_JsonArray                   | .NET 7.0  |  70.52 ms | 2.446 ms | 11.400 ms |  66.32 ms |  98.18 ms |  0.94 |    0.15 | 2250.0000 | 2125.0000 | 1000.0000 | 18361234 B |   57,378.86 |
| Deserialize_Iterate_ToArray             | .NET 7.0  |  76.51 ms | 0.478 ms |  2.601 ms |  76.52 ms |  80.69 ms |  1.02 |    0.04 |  142.8571 |  142.8571 |  142.8571 |  2097950 B |    6,556.09 |
| Serialize_JsonArray_Async               | .NET 7.0  |  22.54 ms | 1.024 ms |  4.606 ms |  19.20 ms |  29.80 ms |  0.30 |    0.06 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |   23,564.13 |
| Deserialize_JsonArray_AsyncEnumerable   | .NET 7.0  |  67.91 ms | 0.384 ms |  1.453 ms |  67.70 ms |  70.42 ms |  0.91 |    0.02 | 2875.0000 | 1125.0000 | 1000.0000 | 15482039 B |   48,381.37 |
|                                         |           |           |          |           |           |           |       |         |           |           |           |            |             |
| Serialize_NdJson_Streaming              | .NET 8.0  |  66.03 ms | 0.794 ms |  4.022 ms |  64.90 ms |  74.55 ms |  1.09 |    0.07 | 1625.0000 |  875.0000 |  875.0000 | 11604654 B |   40,293.94 |
| Serialize_JsonArray                     | .NET 8.0  |  16.51 ms | 0.401 ms |  1.804 ms |  16.00 ms |  20.30 ms |  0.27 |    0.03 |  765.6250 |  750.0000 |  750.0000 |  7540521 B |   26,182.36 |
| Iterate_IAsyncEnumerable                | .NET 8.0  |  60.41 ms | 0.426 ms |  1.549 ms |  60.65 ms |  62.38 ms |  1.00 |    0.04 |         - |         - |         - |      288 B |        1.00 |
| Serialize_NdJson_PipeWriter_Streaming   | .NET 8.0  |  80.10 ms | 0.662 ms |  3.060 ms |  79.95 ms |  85.67 ms |  1.33 |    0.06 | 1000.0000 |  500.0000 |  500.0000 | 11602224 B |   40,285.50 |
| Deserialize_NdJson_Streaming            | .NET 8.0  | 139.14 ms | 0.738 ms |  2.729 ms | 138.82 ms | 143.72 ms |  2.30 |    0.07 | 3000.0000 | 1000.0000 | 1000.0000 | 19536704 B |   67,835.78 |
| Deserialize_NdJson_PipeReader_Streaming | .NET 8.0  | 129.92 ms | 0.703 ms |  2.574 ms | 129.44 ms | 134.61 ms |  2.15 |    0.07 | 3500.0000 | 1000.0000 | 1000.0000 | 19535448 B |   67,831.42 |
| Deserialize_JsonArray                   | .NET 8.0  |  60.31 ms | 0.398 ms |  2.542 ms |  60.27 ms |  64.70 ms |  1.00 |    0.05 | 2300.0000 | 2200.0000 | 1000.0000 | 18361846 B |   63,756.41 |
| Deserialize_Iterate_ToArray             | .NET 8.0  |  62.97 ms | 0.601 ms |  2.437 ms |  63.09 ms |  66.71 ms |  1.04 |    0.05 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |    7,284.66 |
| Serialize_JsonArray_Async               | .NET 8.0  |  16.59 ms | 0.211 ms |  1.003 ms |  16.60 ms |  18.13 ms |  0.27 |    0.02 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |   26,182.37 |
| Deserialize_JsonArray_AsyncEnumerable   | .NET 8.0  |  60.66 ms | 0.509 ms |  1.999 ms |  60.27 ms |  64.73 ms |  1.00 |    0.04 | 2888.8889 | 1000.0000 | 1000.0000 | 15479810 B |   53,749.34 |
|                                         |           |           |          |           |           |           |       |         |           |           |           |            |             |
| Serialize_NdJson_Streaming              | .NET 9.0  |  79.61 ms | 0.753 ms |  4.336 ms |  79.25 ms |  88.32 ms |  1.22 |    0.08 | 2000.0000 | 1000.0000 | 1000.0000 | 13203972 B |   45,847.12 |
| Serialize_JsonArray                     | .NET 9.0  |  14.99 ms | 0.118 ms |  0.585 ms |  14.93 ms |  16.07 ms |  0.23 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540798 B |   26,183.33 |
| Iterate_IAsyncEnumerable                | .NET 9.0  |  65.21 ms | 0.563 ms |  2.480 ms |  65.20 ms |  68.91 ms |  1.00 |    0.05 |         - |         - |         - |      288 B |        1.00 |
| Serialize_NdJson_PipeWriter_Streaming   | .NET 9.0  |  89.44 ms | 2.650 ms | 15.429 ms |  83.48 ms | 131.31 ms |  1.37 |    0.24 |  500.0000 |  500.0000 |  500.0000 |  8390280 B |   29,132.92 |
| Deserialize_NdJson_Streaming            | .NET 9.0  | 150.47 ms | 1.058 ms |  6.040 ms | 149.94 ms | 161.03 ms |  2.31 |    0.13 | 4000.0000 | 1000.0000 | 1000.0000 | 21154600 B |   73,453.47 |
| Deserialize_NdJson_PipeReader_Streaming | .NET 9.0  | 146.52 ms | 1.110 ms |  6.349 ms | 145.86 ms | 157.25 ms |  2.25 |    0.13 | 4000.0000 | 1000.0000 | 1000.0000 | 21157736 B |   73,464.36 |
| Deserialize_JsonArray                   | .NET 9.0  |  59.70 ms | 0.485 ms |  4.203 ms |  59.95 ms |  66.72 ms |  0.92 |    0.07 | 2200.0000 | 2000.0000 | 1000.0000 | 18362304 B |   63,758.00 |
| Deserialize_Iterate_ToArray             | .NET 9.0  |  62.59 ms | 0.591 ms |  2.169 ms |  62.30 ms |  67.51 ms |  0.96 |    0.05 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |    7,284.66 |
| Serialize_JsonArray_Async               | .NET 9.0  |  14.10 ms | 0.250 ms |  1.117 ms |  13.93 ms |  16.50 ms |  0.22 |    0.02 |  765.6250 |  750.0000 |  750.0000 |  7540798 B |   26,183.33 |
| Deserialize_JsonArray_AsyncEnumerable   | .NET 9.0  |  52.46 ms | 1.512 ms |  7.119 ms |  50.34 ms |  68.74 ms |  0.81 |    0.11 | 2666.6667 | 1000.0000 | 1000.0000 | 15481104 B |   53,753.83 |

---

## Interpretation

- **NDJSON streaming** = enumeration cost + write cost  
- **JSON array** is fastest due to single-buffer writes  
- All runtimes improve from **.NET 7 → 10**  
- Memory usage scales with output size (expected for JSON serialization)  
- PipeWriter/PipeReader paths show different characteristics depending on CPU TDP  

---

## Reproduction

Run the benchmark project:

```bash
dotnet run -c Release
```

BenchmarkDotNet builds separate executables for each target runtime.  
The benchmark project targets multiple TFMs to enable cross-runtime comparison.

---

## Notes

This benchmark is intended to show **relative performance characteristics**,  
not absolute throughput numbers.  
Different machines will produce different absolute timings,  
but the relationships between methods remain consistent.
