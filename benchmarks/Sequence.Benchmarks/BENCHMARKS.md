# Juner.Sequence Benchmarks

- **Dataset:** 100,000 items of `MyType`
- **Formats:** NDJSON / JSON array
- **Purpose:** Compare Juner.Sequence streaming vs System.Text.Json buffered JSON.

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
| Method                                        | Job       | Runtime   | Mean      | Error    | StdDev   | Median    | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated  | Alloc Ratio |
|---------------------------------------------- |---------- |---------- |----------:|---------:|---------:|----------:|------:|--------:|----------:|----------:|----------:|-----------:|------------:|
| &#39;03. pure IAsyncEnumerable iteration&#39;         | .NET 10.0 | .NET 10.0 |  33.60 ms | 1.268 ms | 3.739 ms |  33.88 ms |  0.67 |    0.08 |         - |         - |         - |      288 B |        0.90 |
| &#39;08. Convert IAsyncEnumerable to array&#39;       | .NET 10.0 | .NET 10.0 |  37.21 ms | 1.294 ms | 3.814 ms |  37.46 ms |  0.74 |    0.08 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |    6,556.19 |
| &#39;03. pure IAsyncEnumerable iteration&#39;         | .NET 7.0  | .NET 7.0  |  50.09 ms | 0.989 ms | 1.016 ms |  50.02 ms |  1.00 |    0.03 |         - |         - |         - |      320 B |        1.00 |
| &#39;08. Convert IAsyncEnumerable to array&#39;       | .NET 7.0  | .NET 7.0  |  49.95 ms | 0.820 ms | 0.727 ms |  50.12 ms |  1.00 |    0.02 |  181.8182 |  181.8182 |  181.8182 |  2097973 B |    6,556.17 |
| &#39;03. pure IAsyncEnumerable iteration&#39;         | .NET 8.0  | .NET 8.0  |  45.09 ms | 1.294 ms | 3.816 ms |  45.20 ms |  0.90 |    0.08 |         - |         - |         - |      288 B |        0.90 |
| &#39;08. Convert IAsyncEnumerable to array&#39;       | .NET 8.0  | .NET 8.0  |  49.14 ms | 1.100 ms | 3.244 ms |  50.33 ms |  0.98 |    0.07 |  200.0000 |  200.0000 |  200.0000 |  2097952 B |    6,556.10 |
| &#39;03. pure IAsyncEnumerable iteration&#39;         | .NET 9.0  | .NET 9.0  |  42.70 ms | 1.151 ms | 3.394 ms |  42.83 ms |  0.85 |    0.07 |         - |         - |         - |      288 B |        0.90 |
| &#39;08. Convert IAsyncEnumerable to array&#39;       | .NET 9.0  | .NET 9.0  |  47.33 ms | 1.405 ms | 4.141 ms |  48.34 ms |  0.95 |    0.08 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |    6,556.19 |
| &#39;05. NDJSON Deserialize (Stream)&#39;             | .NET 10.0 | .NET 10.0 |  96.00 ms | 1.216 ms | 1.078 ms |  95.90 ms |  1.92 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 21948800 B |   68,590.00 |
| &#39;06. NDJSON Deserialize (PipeReader)&#39;         | .NET 10.0 | .NET 10.0 |  94.64 ms | 1.179 ms | 1.045 ms |  94.84 ms |  1.89 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 21951136 B |   68,597.30 |
| &#39;05. NDJSON Deserialize (Stream)&#39;             | .NET 7.0  | .NET 7.0  | 141.46 ms | 1.698 ms | 1.506 ms | 140.86 ms |  2.83 |    0.06 | 3000.0000 | 1000.0000 | 1000.0000 | 19533296 B |   61,041.55 |
| &#39;06. NDJSON Deserialize (PipeReader)&#39;         | .NET 7.0  | .NET 7.0  | 136.37 ms | 0.840 ms | 0.701 ms | 136.56 ms |  2.72 |    0.06 | 3000.0000 | 1000.0000 | 1000.0000 | 19550856 B |   61,096.43 |
| &#39;05. NDJSON Deserialize (Stream)&#39;             | .NET 8.0  | .NET 8.0  | 124.69 ms | 1.339 ms | 1.187 ms | 124.54 ms |  2.49 |    0.05 | 3000.0000 | 1000.0000 | 1000.0000 | 19537096 B |   61,053.43 |
| &#39;06. NDJSON Deserialize (PipeReader)&#39;         | .NET 8.0  | .NET 8.0  | 119.90 ms | 0.741 ms | 0.657 ms | 120.03 ms |  2.39 |    0.05 | 3000.0000 | 1000.0000 | 1000.0000 | 19538492 B |   61,057.79 |
| &#39;05. NDJSON Deserialize (Stream)&#39;             | .NET 9.0  | .NET 9.0  | 111.01 ms | 1.902 ms | 1.686 ms | 110.50 ms |  2.22 |    0.05 | 3500.0000 | 1000.0000 | 1000.0000 | 21145240 B |   66,078.88 |
| &#39;06. NDJSON Deserialize (PipeReader)&#39;         | .NET 9.0  | .NET 9.0  | 107.99 ms | 1.754 ms | 1.555 ms | 107.81 ms |  2.16 |    0.05 | 3000.0000 | 1000.0000 | 1000.0000 | 21143832 B |   66,074.48 |
| &#39;01. NDJSON Serialize (Stream)&#39;               | .NET 10.0 | .NET 10.0 |  53.27 ms | 0.951 ms | 0.889 ms |  53.26 ms |  1.06 |    0.03 | 2200.0000 | 1000.0000 | 1000.0000 | 14016879 B |   43,802.75 |
| &#39;04. NDJSON Serialize (PipeWriter)&#39;           | .NET 10.0 | .NET 10.0 |  59.78 ms | 0.821 ms | 0.768 ms |  59.68 ms |  1.19 |    0.03 |  888.8889 |  888.8889 |  888.8889 |  8390765 B |   26,221.14 |
| &#39;01. NDJSON Serialize (Stream)&#39;               | .NET 7.0  | .NET 7.0  |  68.73 ms | 0.534 ms | 0.473 ms |  68.76 ms |  1.37 |    0.03 | 1250.0000 |  625.0000 |  625.0000 | 11597364 B |   36,241.76 |
| &#39;04. NDJSON Serialize (PipeWriter)&#39;           | .NET 7.0  | .NET 7.0  |  79.08 ms | 1.305 ms | 1.090 ms |  78.72 ms |  1.58 |    0.04 | 1714.2857 | 1000.0000 | 1000.0000 | 11605672 B |   36,267.72 |
| &#39;01. NDJSON Serialize (Stream)&#39;               | .NET 8.0  | .NET 8.0  |  60.62 ms | 0.538 ms | 0.504 ms |  60.68 ms |  1.21 |    0.03 | 1555.5556 |  888.8889 |  888.8889 | 11607512 B |   36,273.47 |
| &#39;04. NDJSON Serialize (PipeWriter)&#39;           | .NET 8.0  | .NET 8.0  |  69.36 ms | 1.090 ms | 0.966 ms |  69.33 ms |  1.39 |    0.03 | 1500.0000 |  875.0000 |  875.0000 | 11600836 B |   36,252.61 |
| &#39;01. NDJSON Serialize (Stream)&#39;               | .NET 9.0  | .NET 9.0  |  57.99 ms | 1.076 ms | 1.007 ms |  57.95 ms |  1.16 |    0.03 | 1888.8889 |  888.8889 |  888.8889 | 13208146 B |   41,275.46 |
| &#39;04. NDJSON Serialize (PipeWriter)&#39;           | .NET 9.0  | .NET 9.0  |  64.45 ms | 0.850 ms | 0.795 ms |  64.29 ms |  1.29 |    0.03 |  875.0000 |  875.0000 |  875.0000 |  8390748 B |   26,221.09 |
| &#39;07. JSON array Deserialize (non-streaming)&#39;  | .NET 10.0 | .NET 10.0 |  49.64 ms | 0.971 ms | 1.750 ms |  49.27 ms |  0.99 |    0.04 | 2272.7273 | 2000.0000 | 1000.0000 | 18362916 B |   57,384.11 |
| &#39;10. DeserializeAsyncEnumerable (JSON array)&#39; | .NET 10.0 | .NET 10.0 |  37.02 ms | 0.466 ms | 0.413 ms |  36.93 ms |  0.74 |    0.02 | 2642.8571 | 1071.4286 | 1000.0000 | 15480897 B |   48,377.80 |
| &#39;07. JSON array Deserialize (non-streaming)&#39;  | .NET 7.0  | .NET 7.0  |  63.56 ms | 0.766 ms | 0.717 ms |  63.35 ms |  1.27 |    0.03 | 2250.0000 | 2125.0000 | 1000.0000 | 18362152 B |   57,381.72 |
| &#39;10. DeserializeAsyncEnumerable (JSON array)&#39; | .NET 7.0  | .NET 7.0  |  64.81 ms | 0.786 ms | 0.735 ms |  64.62 ms |  1.29 |    0.03 | 2625.0000 | 1250.0000 | 1000.0000 | 15480045 B |   48,375.14 |
| &#39;07. JSON array Deserialize (non-streaming)&#39;  | .NET 8.0  | .NET 8.0  |  56.50 ms | 1.127 ms | 2.198 ms |  56.16 ms |  1.13 |    0.05 | 2250.0000 | 2125.0000 | 1000.0000 | 18361342 B |   57,379.19 |
| &#39;10. DeserializeAsyncEnumerable (JSON array)&#39; | .NET 8.0  | .NET 8.0  |  51.29 ms | 0.375 ms | 0.313 ms |  51.34 ms |  1.02 |    0.02 | 2700.0000 | 1200.0000 | 1000.0000 | 15479830 B |   48,374.47 |
| &#39;07. JSON array Deserialize (non-streaming)&#39;  | .NET 9.0  | .NET 9.0  |  53.59 ms | 1.067 ms | 2.364 ms |  53.50 ms |  1.07 |    0.05 | 2300.0000 | 2200.0000 | 1000.0000 | 18361873 B |   57,380.85 |
| &#39;10. DeserializeAsyncEnumerable (JSON array)&#39; | .NET 9.0  | .NET 9.0  |  41.88 ms | 0.297 ms | 0.263 ms |  41.84 ms |  0.84 |    0.02 | 2666.6667 | 1166.6667 | 1000.0000 | 15483610 B |   48,386.28 |
| &#39;02. JSON array Serialize (non-streaming)&#39;    | .NET 10.0 | .NET 10.0 |  11.19 ms | 0.121 ms | 0.107 ms |  11.21 ms |  0.22 |    0.00 |  750.0000 |  734.3750 |  734.3750 |  7540597 B |   23,564.37 |
| &#39;09. SerializeAsync (JSON array)&#39;             | .NET 10.0 | .NET 10.0 |  11.70 ms | 0.175 ms | 0.155 ms |  11.62 ms |  0.23 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540810 B |   23,565.03 |
| &#39;02. JSON array Serialize (non-streaming)&#39;    | .NET 7.0  | .NET 7.0  |  17.66 ms | 0.212 ms | 0.188 ms |  17.63 ms |  0.35 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |   23,564.13 |
| &#39;09. SerializeAsync (JSON array)&#39;             | .NET 7.0  | .NET 7.0  |  18.04 ms | 0.135 ms | 0.112 ms |  18.04 ms |  0.36 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |   23,564.13 |
| &#39;02. JSON array Serialize (non-streaming)&#39;    | .NET 8.0  | .NET 8.0  |  14.42 ms | 0.073 ms | 0.064 ms |  14.41 ms |  0.29 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540522 B |   23,564.13 |
| &#39;09. SerializeAsync (JSON array)&#39;             | .NET 8.0  | .NET 8.0  |  14.74 ms | 0.133 ms | 0.124 ms |  14.69 ms |  0.29 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540522 B |   23,564.13 |
| &#39;02. JSON array Serialize (non-streaming)&#39;    | .NET 9.0  | .NET 9.0  |  13.18 ms | 0.251 ms | 0.235 ms |  13.16 ms |  0.26 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540798 B |   23,564.99 |
| &#39;09. SerializeAsync (JSON array)&#39;             | .NET 9.0  | .NET 9.0  |  13.17 ms | 0.100 ms | 0.084 ms |  13.18 ms |  0.26 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540798 B |   23,564.99 |

## Reproduction

```bash
dotnet run -f net10.0 -c Release -- -r net7.0 net8.0 net9.0 net10.0 --launchCount 1 --memory
```

BenchmarkDotNet builds separate executables for each target runtime.

---

## Notes

This benchmark shows **relative performance**, not absolute throughput.
