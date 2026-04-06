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
| Type                                 | Method                                                  | Job       | Runtime   | Categories                                            | Mean      | Error    | StdDev   | Median    | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated  | Alloc Ratio |
|------------------------------------- |-------------------------------------------------------- |---------- |---------- |------------------------------------------------------ |----------:|---------:|---------:|----------:|------:|--------:|----------:|----------:|----------:|-----------:|------------:|
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Baseline,JSONArray         |  47.52 ms | 0.496 ms | 0.414 ms |  47.46 ms |  0.75 |    0.02 | 2272.7273 | 2090.9091 | 1000.0000 | 18364231 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Baseline,JSONArray         |  63.38 ms | 1.254 ms | 1.231 ms |  62.93 ms |  1.00 |    0.03 | 2250.0000 | 2125.0000 | 1000.0000 | 18363490 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Baseline,JSONArray         |  56.95 ms | 1.127 ms | 2.172 ms |  56.29 ms |  0.90 |    0.04 | 2222.2222 | 2111.1111 | 1000.0000 | 18364305 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Baseline,JSONArray         |  52.57 ms | 1.179 ms | 3.476 ms |  51.20 ms |  0.83 |    0.06 | 2200.0000 | 1800.0000 | 1000.0000 | 18365616 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      |  97.11 ms | 0.897 ms | 0.749 ms |  97.07 ms |  1.53 |    0.03 | 3500.0000 | 1500.0000 | 1000.0000 | 22140780 B |       1.206 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      |  94.52 ms | 0.641 ms | 0.568 ms |  94.38 ms |  1.49 |    0.03 | 3500.0000 | 1500.0000 | 1000.0000 | 22115980 B |       1.204 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 139.98 ms | 1.783 ms | 1.668 ms | 139.62 ms |  2.21 |    0.05 | 3000.0000 | 1000.0000 | 1000.0000 | 19656880 B |       1.070 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 137.11 ms | 2.218 ms | 2.075 ms | 135.99 ms |  2.16 |    0.05 | 3000.0000 | 1000.0000 | 1000.0000 | 19663600 B |       1.071 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 121.64 ms | 0.853 ms | 0.712 ms | 121.33 ms |  1.92 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 19696364 B |       1.073 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 120.70 ms | 2.330 ms | 2.065 ms | 119.90 ms |  1.91 |    0.05 | 3000.0000 | 1000.0000 | 1000.0000 | 19706936 B |       1.073 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 110.65 ms | 1.063 ms | 0.888 ms | 110.56 ms |  1.75 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 21324120 B |       1.161 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 107.61 ms | 1.828 ms | 1.526 ms | 107.40 ms |  1.70 |    0.04 | 3500.0000 | 1000.0000 | 1000.0000 | 21270920 B |       1.158 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  37.88 ms | 0.542 ms | 0.481 ms |  37.73 ms |  0.60 |    0.01 | 2642.8571 | 1142.8571 | 1000.0000 | 15482913 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  64.12 ms | 0.995 ms | 0.930 ms |  63.90 ms |  1.01 |    0.02 | 2625.0000 | 1125.0000 | 1000.0000 | 15481245 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  53.61 ms | 0.842 ms | 0.787 ms |  53.29 ms |  0.85 |    0.02 | 2700.0000 | 1100.0000 | 1000.0000 | 15481718 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  42.90 ms | 0.828 ms | 0.986 ms |  42.88 ms |  0.68 |    0.02 | 2600.0000 | 1000.0000 | 1000.0000 | 15484123 B |       0.843 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 10.0 | .NET 10.0 | Deserialize,Baseline                                  |  34.97 ms | 1.305 ms | 3.847 ms |  34.58 ms |  0.55 |    0.06 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 7.0  | .NET 7.0  | Deserialize,Baseline                                  |  48.22 ms | 0.924 ms | 1.233 ms |  47.81 ms |  0.76 |    0.02 |  181.8182 |  181.8182 |  181.8182 |  2097973 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 8.0  | .NET 8.0  | Deserialize,Baseline                                  |  49.38 ms | 0.982 ms | 2.517 ms |  50.03 ms |  0.78 |    0.04 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 9.0  | .NET 9.0  | Deserialize,Baseline                                  |  44.59 ms | 1.355 ms | 3.995 ms |  45.59 ms |  0.70 |    0.06 |  230.7692 |  230.7692 |  230.7692 |  2097970 B |       0.114 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 10.0 | .NET 10.0 | Deserialize,System.Text.Json,JSONArray                |  49.37 ms | 0.976 ms | 2.100 ms |  48.89 ms |  0.78 |    0.04 | 2300.0000 | 2100.0000 | 1000.0000 | 18364826 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 7.0  | .NET 7.0  | Deserialize,System.Text.Json,JSONArray                |  63.97 ms | 0.766 ms | 0.716 ms |  64.04 ms |  1.01 |    0.02 | 2250.0000 | 2125.0000 | 1000.0000 | 18363630 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 8.0  | .NET 8.0  | Deserialize,System.Text.Json,JSONArray                |  55.57 ms | 1.053 ms | 1.511 ms |  55.91 ms |  0.88 |    0.03 | 2250.0000 | 2125.0000 | 1000.0000 | 18362967 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 9.0  | .NET 9.0  | Deserialize,System.Text.Json,JSONArray                |  52.26 ms | 1.042 ms | 2.745 ms |  51.00 ms |  0.82 |    0.05 | 2250.0000 | 1750.0000 | 1000.0000 | 18365844 B |       1.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 10.0 | .NET 10.0 | Serialize,Baseline                                    |  38.88 ms | 1.374 ms | 4.053 ms |  39.69 ms |  0.61 |    0.06 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 7.0  | .NET 7.0  | Serialize,Baseline                                    |  47.90 ms | 0.949 ms | 1.712 ms |  47.87 ms |  0.76 |    0.03 |         - |         - |         - |      320 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 8.0  | .NET 8.0  | Serialize,Baseline                                    |  41.80 ms | 1.199 ms | 3.534 ms |  41.27 ms |  0.66 |    0.06 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 9.0  | .NET 9.0  | Serialize,Baseline                                    |  43.19 ms | 1.401 ms | 4.130 ms |  44.29 ms |  0.68 |    0.07 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,NDJSON                       |  53.33 ms | 0.573 ms | 0.478 ms |  53.19 ms |  0.84 |    0.02 | 1666.6667 |  666.6667 |  666.6667 | 14119883 B |       0.769 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,NDJSON                       |  58.97 ms | 0.274 ms | 0.229 ms |  58.92 ms |  0.93 |    0.02 |  888.8889 |  888.8889 |  888.8889 |  8420063 B |       0.459 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,NDJSON                       |  68.56 ms | 1.211 ms | 1.073 ms |  68.32 ms |  1.08 |    0.03 | 1500.0000 | 1000.0000 |  875.0000 | 11708381 B |       0.638 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,NDJSON                       |  78.94 ms | 1.185 ms | 1.108 ms |  79.22 ms |  1.25 |    0.03 | 1714.2857 | 1142.8571 | 1000.0000 | 11709079 B |       0.638 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,NDJSON                       |  61.58 ms | 1.194 ms | 1.674 ms |  61.61 ms |  0.97 |    0.03 | 1333.3333 |  777.7778 |  666.6667 | 11690269 B |       0.637 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,NDJSON                       |  67.12 ms | 1.013 ms | 0.947 ms |  66.93 ms |  1.06 |    0.02 | 1500.0000 | 1000.0000 |  875.0000 | 11685813 B |       0.636 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,NDJSON                       |  57.20 ms | 1.040 ms | 0.972 ms |  56.79 ms |  0.90 |    0.02 | 1888.8889 | 1111.1111 |  888.8889 | 13319746 B |       0.725 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,NDJSON                       |  64.70 ms | 0.479 ms | 0.400 ms |  64.77 ms |  1.02 |    0.02 |  666.6667 |  666.6667 |  666.6667 |  8415208 B |       0.458 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 10.0 | .NET 10.0 | Serialize,System.Text.Json,JSONArray                  |  11.17 ms | 0.184 ms | 0.163 ms |  11.16 ms |  0.18 |    0.00 |  718.7500 |  703.1250 |  703.1250 |  7540823 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 10.0 | .NET 10.0 | Serialize,System.Text.Json,JSONArray                  |  11.94 ms | 0.221 ms | 0.207 ms |  11.84 ms |  0.19 |    0.00 |  750.0000 |  734.3750 |  734.3750 |  7541050 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 7.0  | .NET 7.0  | Serialize,System.Text.Json,JSONArray                  |  17.86 ms | 0.229 ms | 0.203 ms |  17.90 ms |  0.28 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7541035 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 7.0  | .NET 7.0  | Serialize,System.Text.Json,JSONArray                  |  17.98 ms | 0.217 ms | 0.203 ms |  17.96 ms |  0.28 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7541035 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 8.0  | .NET 8.0  | Serialize,System.Text.Json,JSONArray                  |  16.00 ms | 0.224 ms | 0.209 ms |  16.00 ms |  0.25 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540778 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 8.0  | .NET 8.0  | Serialize,System.Text.Json,JSONArray                  |  14.87 ms | 0.132 ms | 0.124 ms |  14.89 ms |  0.23 |    0.00 |  750.0000 |  750.0000 |  750.0000 |  7541035 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 9.0  | .NET 9.0  | Serialize,System.Text.Json,JSONArray                  |  13.20 ms | 0.208 ms | 0.194 ms |  13.19 ms |  0.21 |    0.00 |  750.0000 |  734.3750 |  734.3750 |  7541036 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 9.0  | .NET 9.0  | Serialize,System.Text.Json,JSONArray                  |  13.01 ms | 0.179 ms | 0.150 ms |  12.99 ms |  0.21 |    0.00 |  765.6250 |  750.0000 |  750.0000 |  7541054 B |       0.411 |

## Reproduction

```bash
dotnet run -f net10.0 -c Release -- -r net7.0 net8.0 net9.0 net10.0 --launchCount 1 --memory
```

BenchmarkDotNet builds separate executables for each target runtime.

---

## Notes

This benchmark shows **relative performance**, not absolute throughput.
