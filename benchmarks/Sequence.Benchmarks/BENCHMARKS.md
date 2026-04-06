# Juner.Sequence Benchmarks

- **Dataset:** 100,000 items of `MyType`
- **Formats:** NDJSON / JSON array
- **Purpose:** Compare Juner.Sequence streaming vs System.Text.Json buffered JSON.

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
| Type                                 | Method                                                  | Job       | Runtime   | Categories                                            | Mean      | Error    | StdDev    | Median    | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated  | Alloc Ratio |
|------------------------------------- |-------------------------------------------------------- |---------- |---------- |------------------------------------------------------ |----------:|---------:|----------:|----------:|------:|--------:|----------:|----------:|----------:|-----------:|------------:|
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Baseline,JSONArray         |  51.24 ms | 1.131 ms |  3.226 ms |  50.96 ms |  0.74 |    0.06 | 2300.0000 | 2200.0000 | 1000.0000 | 18361906 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Baseline,JSONArray         |  69.59 ms | 1.378 ms |  3.110 ms |  68.33 ms |  1.00 |    0.06 | 2250.0000 | 2125.0000 | 1000.0000 | 18361243 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Baseline,JSONArray         |  61.36 ms | 1.216 ms |  3.096 ms |  60.57 ms |  0.88 |    0.06 | 2300.0000 | 2200.0000 | 1000.0000 | 18359671 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Baseline,JSONArray         |  57.38 ms | 2.063 ms |  6.084 ms |  57.97 ms |  0.83 |    0.09 | 2250.0000 | 1750.0000 | 1000.0000 | 18361288 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 113.33 ms | 2.247 ms |  5.719 ms | 114.06 ms |  1.63 |    0.11 | 4000.0000 | 1000.0000 | 1000.0000 | 21948116 B |       1.195 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 119.62 ms | 4.486 ms | 12.870 ms | 116.02 ms |  1.72 |    0.20 | 4000.0000 | 1000.0000 | 1000.0000 | 21952296 B |       1.196 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 151.26 ms | 2.439 ms |  2.281 ms | 150.83 ms |  2.18 |    0.10 | 3000.0000 | 1000.0000 | 1000.0000 | 19531984 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 151.11 ms | 3.363 ms |  9.810 ms | 148.82 ms |  2.18 |    0.17 | 3500.0000 | 1000.0000 | 1000.0000 | 19538644 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 139.35 ms | 2.907 ms |  8.247 ms | 137.48 ms |  2.01 |    0.15 | 3000.0000 | 1000.0000 | 1000.0000 | 19536664 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 132.72 ms | 1.992 ms |  1.957 ms | 132.39 ms |  1.91 |    0.09 | 3000.0000 | 1000.0000 | 1000.0000 | 19532520 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 129.09 ms | 3.174 ms |  9.003 ms | 127.07 ms |  1.86 |    0.15 | 4000.0000 | 1000.0000 | 1000.0000 | 21157672 B |       1.152 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON      | 132.52 ms | 5.176 ms | 15.181 ms | 127.35 ms |  1.91 |    0.23 | 4000.0000 | 1000.0000 | 1000.0000 | 21147776 B |       1.152 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  36.72 ms | 0.727 ms |  1.235 ms |  36.55 ms |  0.53 |    0.03 | 2923.0769 | 1076.9231 | 1000.0000 | 15481921 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  68.43 ms | 1.324 ms |  1.174 ms |  68.55 ms |  0.99 |    0.04 | 2857.1429 | 1142.8571 | 1000.0000 | 15481823 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  53.40 ms | 0.954 ms |  0.745 ms |  53.20 ms |  0.77 |    0.03 | 2900.0000 | 1100.0000 | 1000.0000 | 15480132 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray |  43.29 ms | 0.592 ms |  0.495 ms |  43.52 ms |  0.62 |    0.03 | 2916.6667 | 1083.3333 | 1000.0000 | 15480886 B |       0.843 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 10.0 | .NET 10.0 | Deserialize,Baseline                                  |  58.06 ms | 0.607 ms |  0.567 ms |  58.00 ms |  0.84 |    0.04 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 7.0  | .NET 7.0  | Deserialize,Baseline                                  |  71.29 ms | 1.339 ms |  1.315 ms |  71.69 ms |  1.03 |    0.05 |  250.0000 |  250.0000 |  250.0000 |  2098014 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 8.0  | .NET 8.0  | Deserialize,Baseline                                  |  60.92 ms | 1.120 ms |  1.048 ms |  60.86 ms |  0.88 |    0.04 |  250.0000 |  250.0000 |  250.0000 |  2097982 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 9.0  | .NET 9.0  | Deserialize,Baseline                                  |  62.65 ms | 1.090 ms |  1.019 ms |  62.95 ms |  0.90 |    0.04 |  222.2222 |  222.2222 |  222.2222 |  2097965 B |       0.114 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 10.0 | .NET 10.0 | Deserialize,System.Text.Json,JSONArray                |  58.54 ms | 4.478 ms | 13.205 ms |  53.38 ms |  0.84 |    0.19 | 2200.0000 | 2000.0000 | 1000.0000 | 18363427 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 7.0  | .NET 7.0  | Deserialize,System.Text.Json,JSONArray                |  71.37 ms | 1.513 ms |  4.267 ms |  69.78 ms |  1.03 |    0.08 | 2285.7143 | 2142.8571 | 1000.0000 | 18360736 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 8.0  | .NET 8.0  | Deserialize,System.Text.Json,JSONArray                |  59.62 ms | 1.181 ms |  2.593 ms |  59.47 ms |  0.86 |    0.05 | 2222.2222 | 2111.1111 | 1000.0000 | 18361749 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 9.0  | .NET 9.0  | Deserialize,System.Text.Json,JSONArray                |  57.58 ms | 1.306 ms |  3.852 ms |  57.85 ms |  0.83 |    0.07 | 2222.2222 | 2111.1111 | 1000.0000 | 18362399 B |       1.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 10.0 | .NET 10.0 | Serialize,Baseline                                    |  57.33 ms | 1.117 ms |  1.491 ms |  57.56 ms |  0.83 |    0.04 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 7.0  | .NET 7.0  | Serialize,Baseline                                    |  63.96 ms | 1.020 ms |  0.904 ms |  63.63 ms |  0.92 |    0.04 |         - |         - |         - |      320 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 8.0  | .NET 8.0  | Serialize,Baseline                                    |  58.94 ms | 1.105 ms |  1.034 ms |  59.33 ms |  0.85 |    0.04 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 9.0  | .NET 9.0  | Serialize,Baseline                                    |  58.77 ms | 1.128 ms |  1.385 ms |  58.94 ms |  0.85 |    0.04 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,NDJSON                       |  79.07 ms | 5.395 ms | 15.908 ms |  72.29 ms |  1.14 |    0.23 | 2333.3333 | 1000.0000 | 1000.0000 | 14011243 B |       0.763 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,NDJSON                       |  77.76 ms | 4.710 ms | 13.815 ms |  71.83 ms |  1.12 |    0.20 |         - |         - |         - |  8389656 B |       0.457 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,NDJSON                       |  90.69 ms | 4.285 ms | 12.156 ms |  88.12 ms |  1.31 |    0.18 | 1500.0000 |  833.3333 |  833.3333 | 11611605 B |       0.632 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,NDJSON                       |  99.35 ms | 2.871 ms |  7.861 ms |  96.81 ms |  1.43 |    0.13 | 1500.0000 |  833.3333 |  833.3333 | 11603997 B |       0.632 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,NDJSON                       |  90.71 ms | 5.230 ms | 14.751 ms |  84.19 ms |  1.31 |    0.22 | 1333.3333 |  666.6667 |  666.6667 | 11599235 B |       0.632 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,NDJSON                       |  96.11 ms | 3.720 ms | 10.430 ms |  93.35 ms |  1.38 |    0.16 | 1000.0000 |  500.0000 |  500.0000 | 11599224 B |       0.632 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,NDJSON                       |  77.57 ms | 1.676 ms |  4.559 ms |  76.61 ms |  1.12 |    0.08 | 2000.0000 | 1000.0000 | 1000.0000 | 13221184 B |       0.720 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,NDJSON                       |  75.23 ms | 1.470 ms |  1.227 ms |  74.62 ms |  1.08 |    0.05 |         - |         - |         - |  8389656 B |       0.457 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 10.0 | .NET 10.0 | Serialize,System.Text.Json,JSONArray                  |  10.52 ms | 0.207 ms |  0.203 ms |  10.47 ms |  0.15 |    0.01 |  687.5000 |  671.8750 |  671.8750 |  7540534 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 10.0 | .NET 10.0 | Serialize,System.Text.Json,JSONArray                  |  10.37 ms | 0.148 ms |  0.124 ms |  10.38 ms |  0.15 |    0.01 |  687.5000 |  671.8750 |  671.8750 |  7540725 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 7.0  | .NET 7.0  | Serialize,System.Text.Json,JSONArray                  |  19.21 ms | 0.262 ms |  0.245 ms |  19.19 ms |  0.28 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 7.0  | .NET 7.0  | Serialize,System.Text.Json,JSONArray                  |  18.61 ms | 0.360 ms |  0.385 ms |  18.49 ms |  0.27 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540522 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 8.0  | .NET 8.0  | Serialize,System.Text.Json,JSONArray                  |  15.21 ms | 0.206 ms |  0.192 ms |  15.22 ms |  0.22 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540521 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 8.0  | .NET 8.0  | Serialize,System.Text.Json,JSONArray                  |  14.23 ms | 0.181 ms |  0.160 ms |  14.18 ms |  0.20 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540522 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 9.0  | .NET 9.0  | Serialize,System.Text.Json,JSONArray                  |  12.59 ms | 0.109 ms |  0.091 ms |  12.61 ms |  0.18 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540795 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 9.0  | .NET 9.0  | Serialize,System.Text.Json,JSONArray                  |  12.75 ms | 0.110 ms |  0.098 ms |  12.75 ms |  0.18 |    0.01 |  765.6250 |  750.0000 |  750.0000 |  7540796 B |       0.411 |

## Reproduction

```bash
dotnet run -f net10.0 -c Release -- -r net7.0 net8.0 net9.0 net10.0 --launchCount 1 --memory
```

BenchmarkDotNet builds separate executables for each target runtime.

---

## Notes

This benchmark shows **relative performance**, not absolute throughput.
