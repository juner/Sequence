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
| Type                                 | Method                                                  | Job       | Runtime   | Categories                                             | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated  | Alloc Ratio |
|------------------------------------- |-------------------------------------------------------- |---------- |---------- |------------------------------------------------------- |----------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|-----------:|------------:|
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Baseline,JSONArray          |  48.82 ms | 0.973 ms | 1.457 ms |  0.77 |    0.02 | 2300.0000 | 2100.0000 | 1000.0000 | 18361848 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Baseline,JSONArray          |  63.69 ms | 0.715 ms | 0.634 ms |  1.00 |    0.01 | 2250.0000 | 2125.0000 | 1000.0000 | 18360751 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Baseline,JSONArray          |  61.30 ms | 1.189 ms | 2.731 ms |  0.96 |    0.04 | 2300.0000 | 2200.0000 | 1000.0000 | 18360082 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;00. Baseline — JSON array Deserialize (non-streaming)&#39; | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Baseline,JSONArray          |  56.88 ms | 1.127 ms | 2.426 ms |  0.89 |    0.04 | 2300.0000 | 2200.0000 | 1000.0000 | 18363222 B |       1.000 |
| BenchmarksDeserializeAsyncEnumerable | &#39;04. JSON Lines Deserialize (Stream)&#39;                   | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    |  97.49 ms | 1.127 ms | 0.999 ms |  1.53 |    0.02 | 3000.0000 | 1000.0000 | 1000.0000 | 21958472 B |       1.196 |
| BenchmarksDeserializeAsyncEnumerable | &#39;05. JSON Lines Deserialize (PipeReader)&#39;               | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    |  95.06 ms | 1.554 ms | 1.377 ms |  1.49 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 21947880 B |       1.195 |
| BenchmarksDeserializeAsyncEnumerable | &#39;04. JSON Lines Deserialize (Stream)&#39;                   | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    | 144.06 ms | 2.103 ms | 1.967 ms |  2.26 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 19544688 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;05. JSON Lines Deserialize (PipeReader)&#39;               | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    | 135.03 ms | 1.586 ms | 1.406 ms |  2.12 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 19535448 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;04. JSON Lines Deserialize (Stream)&#39;                   | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    | 125.52 ms | 2.472 ms | 2.065 ms |  1.97 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 19534400 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;05. JSON Lines Deserialize (PipeReader)&#39;               | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    | 118.05 ms | 2.083 ms | 1.847 ms |  1.85 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 19538432 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;04. JSON Lines Deserialize (Stream)&#39;                   | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    | 111.24 ms | 2.114 ms | 2.171 ms |  1.75 |    0.04 | 3500.0000 | 1000.0000 | 1000.0000 | 21143992 B |       1.152 |
| BenchmarksDeserializeAsyncEnumerable | &#39;05. JSON Lines Deserialize (PipeReader)&#39;               | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONLines    | 109.60 ms | 1.803 ms | 1.687 ms |  1.72 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 21148736 B |       1.152 |
| BenchmarksDeserializeAsyncEnumerable | &#39;06. JSON Sequence Deserialize (Stream)&#39;                | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence |  99.28 ms | 1.286 ms | 1.202 ms |  1.56 |    0.02 | 3000.0000 | 1000.0000 | 1000.0000 | 21953784 B |       1.196 |
| BenchmarksDeserializeAsyncEnumerable | &#39;07. JSON Sequence Deserialize (PipeReader)&#39;            | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence |  94.85 ms | 0.829 ms | 0.735 ms |  1.49 |    0.02 | 3500.0000 | 1000.0000 | 1000.0000 | 21942988 B |       1.195 |
| BenchmarksDeserializeAsyncEnumerable | &#39;06. JSON Sequence Deserialize (Stream)&#39;                | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence | 143.91 ms | 2.851 ms | 2.800 ms |  2.26 |    0.05 | 3000.0000 | 1000.0000 | 1000.0000 | 19531008 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;07. JSON Sequence Deserialize (PipeReader)&#39;            | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence | 139.86 ms | 1.755 ms | 1.465 ms |  2.20 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 19536608 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;06. JSON Sequence Deserialize (Stream)&#39;                | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence | 124.38 ms | 1.898 ms | 1.682 ms |  1.95 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 19530800 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;07. JSON Sequence Deserialize (PipeReader)&#39;            | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence | 121.73 ms | 2.250 ms | 1.995 ms |  1.91 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 19529784 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;06. JSON Sequence Deserialize (Stream)&#39;                | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence | 115.96 ms | 2.303 ms | 2.155 ms |  1.82 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 21159464 B |       1.152 |
| BenchmarksDeserializeAsyncEnumerable | &#39;07. JSON Sequence Deserialize (PipeReader)&#39;            | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,JSONSequence | 115.72 ms | 1.885 ms | 1.574 ms |  1.82 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 21141872 B |       1.151 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       |  97.60 ms | 1.440 ms | 1.276 ms |  1.53 |    0.02 | 3000.0000 | 1000.0000 | 1000.0000 | 21956704 B |       1.196 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       |  95.23 ms | 1.795 ms | 1.763 ms |  1.50 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 21953120 B |       1.196 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       | 139.78 ms | 1.057 ms | 0.937 ms |  2.19 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 19538248 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       | 135.32 ms | 1.402 ms | 1.312 ms |  2.12 |    0.03 | 3250.0000 | 1000.0000 | 1000.0000 | 19540292 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       | 122.40 ms | 2.031 ms | 1.696 ms |  1.92 |    0.03 | 3000.0000 | 1000.0000 | 1000.0000 | 19534208 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       | 118.71 ms | 2.244 ms | 2.099 ms |  1.86 |    0.04 | 3000.0000 | 1000.0000 | 1000.0000 | 19531704 B |       1.064 |
| BenchmarksDeserializeAsyncEnumerable | &#39;01. NDJSON Deserialize (Stream)&#39;                       | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       | 111.46 ms | 0.736 ms | 0.652 ms |  1.75 |    0.02 | 3500.0000 | 1000.0000 | 1000.0000 | 21148520 B |       1.152 |
| BenchmarksDeserializeAsyncEnumerable | &#39;02. NDJSON Deserialize (PipeReader)&#39;                   | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,Juner.Sequence,NDJSON       | 106.65 ms | 0.710 ms | 0.555 ms |  1.67 |    0.02 | 3000.0000 | 1000.0000 | 1000.0000 | 21146200 B |       1.152 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 10.0 | .NET 10.0 | DeserializeAsyncEnumerable,System.Text.Json,JSONArray  |  37.71 ms | 0.680 ms | 0.975 ms |  0.59 |    0.02 | 2642.8571 | 1214.2857 | 1000.0000 | 15481398 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 7.0  | .NET 7.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray  |  64.33 ms | 0.323 ms | 0.252 ms |  1.01 |    0.01 | 2625.0000 | 1250.0000 | 1000.0000 | 15481874 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 8.0  | .NET 8.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray  |  51.80 ms | 0.377 ms | 0.315 ms |  0.81 |    0.01 | 2700.0000 | 1100.0000 | 1000.0000 | 15479976 B |       0.843 |
| BenchmarksDeserializeAsyncEnumerable | &#39;03. DeserializeAsyncEnumerable (JSON array)&#39;           | .NET 9.0  | .NET 9.0  | DeserializeAsyncEnumerable,System.Text.Json,JSONArray  |  41.86 ms | 0.550 ms | 0.459 ms |  0.66 |    0.01 | 2692.3077 | 1230.7692 | 1000.0000 | 15481796 B |       0.843 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 10.0 | .NET 10.0 | Deserialize,Baseline                                   |  39.98 ms | 1.665 ms | 4.911 ms |  0.63 |    0.08 |  230.7692 |  230.7692 |  230.7692 |  2097970 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 7.0  | .NET 7.0  | Deserialize,Baseline                                   |  51.02 ms | 1.009 ms | 1.844 ms |  0.80 |    0.03 |  200.0000 |  200.0000 |  200.0000 |  2097984 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 8.0  | .NET 8.0  | Deserialize,Baseline                                   |  43.82 ms | 0.944 ms | 2.785 ms |  0.69 |    0.04 |  181.8182 |  181.8182 |  181.8182 |  2097941 B |       0.114 |
| BenchmarksDeserialize                | &#39;00. Baseline — Convert IAsyncEnumerable to array&#39;      | .NET 9.0  | .NET 9.0  | Deserialize,Baseline                                   |  45.24 ms | 1.566 ms | 4.619 ms |  0.71 |    0.07 |  230.7692 |  230.7692 |  230.7692 |  2097970 B |       0.114 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 10.0 | .NET 10.0 | Deserialize,System.Text.Json,JSONArray                 |  47.85 ms | 0.922 ms | 1.025 ms |  0.75 |    0.02 | 2272.7273 | 2000.0000 | 1000.0000 | 18361298 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 7.0  | .NET 7.0  | Deserialize,System.Text.Json,JSONArray                 |  63.55 ms | 0.779 ms | 0.651 ms |  1.00 |    0.01 | 2250.0000 | 2125.0000 | 1000.0000 | 18360736 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 8.0  | .NET 8.0  | Deserialize,System.Text.Json,JSONArray                 |  56.58 ms | 0.967 ms | 0.857 ms |  0.89 |    0.02 | 2222.2222 | 2111.1111 | 1000.0000 | 18360261 B |       1.000 |
| BenchmarksDeserialize                | &#39;01. JSON array Deserialize (non-streaming)&#39;            | .NET 9.0  | .NET 9.0  | Deserialize,System.Text.Json,JSONArray                 |  55.86 ms | 1.033 ms | 1.940 ms |  0.88 |    0.03 | 2300.0000 | 2200.0000 | 1000.0000 | 18362844 B |       1.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 10.0 | .NET 10.0 | Serialize,Baseline                                     |  34.57 ms | 1.479 ms | 4.360 ms |  0.54 |    0.07 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 7.0  | .NET 7.0  | Serialize,Baseline                                     |  49.62 ms | 0.992 ms | 1.018 ms |  0.78 |    0.02 |         - |         - |         - |      320 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 8.0  | .NET 8.0  | Serialize,Baseline                                     |  45.54 ms | 1.276 ms | 3.761 ms |  0.72 |    0.06 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;00. Baseline — pure IAsyncEnumerable iteration&#39;        | .NET 9.0  | .NET 9.0  | Serialize,Baseline                                     |  41.17 ms | 1.578 ms | 4.652 ms |  0.65 |    0.07 |         - |         - |         - |      288 B |       0.000 |
| BenchmarksSerialize                  | &#39;05. JSON Lines Serialize (Stream)&#39;                     | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,JSONLines                     |  52.07 ms | 0.849 ms | 0.709 ms |  0.82 |    0.01 | 1666.6667 |  666.6667 |  666.6667 | 14002373 B |       0.763 |
| BenchmarksSerialize                  | &#39;06. JSON Lines Serialize (PipeWriter)&#39;                 | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,JSONLines                     |  57.97 ms | 0.694 ms | 0.542 ms |  0.91 |    0.01 |  888.8889 |  888.8889 |  888.8889 |  8390768 B |       0.457 |
| BenchmarksSerialize                  | &#39;05. JSON Lines Serialize (Stream)&#39;                     | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,JSONLines                     |  71.00 ms | 1.365 ms | 2.243 ms |  1.11 |    0.04 | 1250.0000 |  625.0000 |  625.0000 | 11598498 B |       0.632 |
| BenchmarksSerialize                  | &#39;06. JSON Lines Serialize (PipeWriter)&#39;                 | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,JSONLines                     |  78.26 ms | 1.385 ms | 1.295 ms |  1.23 |    0.02 | 1714.2857 | 1000.0000 | 1000.0000 | 11607813 B |       0.632 |
| BenchmarksSerialize                  | &#39;05. JSON Lines Serialize (Stream)&#39;                     | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,JSONLines                     |  59.02 ms | 1.133 ms | 1.060 ms |  0.93 |    0.02 | 1555.5556 |  888.8889 |  888.8889 | 11603307 B |       0.632 |
| BenchmarksSerialize                  | &#39;06. JSON Lines Serialize (PipeWriter)&#39;                 | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,JSONLines                     |  68.28 ms | 1.019 ms | 0.953 ms |  1.07 |    0.02 | 1000.0000 |  500.0000 |  500.0000 | 11603568 B |       0.632 |
| BenchmarksSerialize                  | &#39;05. JSON Lines Serialize (Stream)&#39;                     | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,JSONLines                     |  60.56 ms | 1.195 ms | 1.118 ms |  0.95 |    0.02 | 1888.8889 |  888.8889 |  888.8889 | 13213064 B |       0.720 |
| BenchmarksSerialize                  | &#39;06. JSON Lines Serialize (PipeWriter)&#39;                 | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,JSONLines                     |  65.36 ms | 0.986 ms | 0.874 ms |  1.03 |    0.02 |  500.0000 |  500.0000 |  500.0000 |  8390280 B |       0.457 |
| BenchmarksSerialize                  | &#39;07. JSON Sequence Serialize (Stream)&#39;                  | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,JSONSequence                  |  54.98 ms | 0.842 ms | 0.746 ms |  0.86 |    0.01 | 2222.2222 | 1000.0000 | 1000.0000 | 14010230 B |       0.763 |
| BenchmarksSerialize                  | &#39;08. JSON Sequence Serialize (PipeWriter)&#39;              | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,JSONSequence                  |  62.72 ms | 0.485 ms | 0.430 ms |  0.98 |    0.01 |  666.6667 |  666.6667 |  666.6667 |  8390488 B |       0.457 |
| BenchmarksSerialize                  | &#39;07. JSON Sequence Serialize (Stream)&#39;                  | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,JSONSequence                  |  70.11 ms | 1.337 ms | 1.251 ms |  1.10 |    0.02 | 1250.0000 |  625.0000 |  625.0000 | 11597793 B |       0.632 |
| BenchmarksSerialize                  | &#39;08. JSON Sequence Serialize (PipeWriter)&#39;              | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,JSONSequence                  |  99.45 ms | 1.058 ms | 0.883 ms |  1.56 |    0.02 | 1500.0000 |  833.3333 |  833.3333 | 11609001 B |       0.632 |
| BenchmarksSerialize                  | &#39;07. JSON Sequence Serialize (Stream)&#39;                  | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,JSONSequence                  |  61.93 ms | 0.322 ms | 0.301 ms |  0.97 |    0.01 | 1555.5556 |  888.8889 |  888.8889 | 11611940 B |       0.632 |
| BenchmarksSerialize                  | &#39;08. JSON Sequence Serialize (PipeWriter)&#39;              | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,JSONSequence                  |  77.65 ms | 1.015 ms | 0.950 ms |  1.22 |    0.02 | 1714.2857 | 1000.0000 | 1000.0000 | 11605506 B |       0.632 |
| BenchmarksSerialize                  | &#39;07. JSON Sequence Serialize (Stream)&#39;                  | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,JSONSequence                  |  59.47 ms | 1.153 ms | 1.761 ms |  0.93 |    0.03 | 2000.0000 | 1000.0000 | 1000.0000 | 13203480 B |       0.719 |
| BenchmarksSerialize                  | &#39;08. JSON Sequence Serialize (PipeWriter)&#39;              | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,JSONSequence                  |  67.89 ms | 0.738 ms | 0.616 ms |  1.07 |    0.01 |  875.0000 |  875.0000 |  875.0000 |  8390751 B |       0.457 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,NDJSON                        |  51.57 ms | 0.390 ms | 0.326 ms |  0.81 |    0.01 | 1666.6667 |  666.6667 |  666.6667 | 14004907 B |       0.763 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 10.0 | .NET 10.0 | Serialize,Juner.Sequence,NDJSON                        |  59.62 ms | 1.145 ms | 1.176 ms |  0.94 |    0.02 |  888.8889 |  888.8889 |  888.8889 |  8390765 B |       0.457 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,NDJSON                        |  67.05 ms | 1.075 ms | 0.898 ms |  1.05 |    0.02 | 1714.2857 | 1000.0000 | 1000.0000 | 11605269 B |       0.632 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 7.0  | .NET 7.0  | Serialize,Juner.Sequence,NDJSON                        |  79.16 ms | 0.941 ms | 0.786 ms |  1.24 |    0.02 | 1714.2857 | 1000.0000 | 1000.0000 | 11613701 B |       0.633 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,NDJSON                        |  62.39 ms | 0.996 ms | 0.883 ms |  0.98 |    0.02 | 1555.5556 |  888.8889 |  888.8889 | 11616015 B |       0.633 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 8.0  | .NET 8.0  | Serialize,Juner.Sequence,NDJSON                        |  70.33 ms | 1.202 ms | 1.125 ms |  1.10 |    0.02 | 1250.0000 |  625.0000 |  625.0000 | 11600190 B |       0.632 |
| BenchmarksSerialize                  | &#39;01. NDJSON Serialize (Stream)&#39;                         | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,NDJSON                        |  59.79 ms | 0.767 ms | 1.324 ms |  0.94 |    0.02 | 2000.0000 | 1000.0000 | 1000.0000 | 13205739 B |       0.719 |
| BenchmarksSerialize                  | &#39;03. NDJSON Serialize (PipeWriter)&#39;                     | .NET 9.0  | .NET 9.0  | Serialize,Juner.Sequence,NDJSON                        |  66.41 ms | 0.394 ms | 0.369 ms |  1.04 |    0.01 |  500.0000 |  500.0000 |  500.0000 |  8390280 B |       0.457 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 10.0 | .NET 10.0 | Serialize,System.Text.Json,JSONArray                   |  12.19 ms | 0.081 ms | 0.072 ms |  0.19 |    0.00 |  687.5000 |  671.8750 |  671.8750 |  7540533 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 10.0 | .NET 10.0 | Serialize,System.Text.Json,JSONArray                   |  12.78 ms | 0.163 ms | 0.152 ms |  0.20 |    0.00 |  687.5000 |  671.8750 |  671.8750 |  7540726 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 7.0  | .NET 7.0  | Serialize,System.Text.Json,JSONArray                   |  18.54 ms | 0.358 ms | 0.335 ms |  0.29 |    0.01 |  750.0000 |  750.0000 |  750.0000 |  7540521 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 7.0  | .NET 7.0  | Serialize,System.Text.Json,JSONArray                   |  18.78 ms | 0.187 ms | 0.166 ms |  0.29 |    0.00 |  718.7500 |  718.7500 |  718.7500 |  7540499 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 8.0  | .NET 8.0  | Serialize,System.Text.Json,JSONArray                   |  15.74 ms | 0.232 ms | 0.268 ms |  0.25 |    0.00 |  750.0000 |  734.3750 |  734.3750 |  7540509 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 8.0  | .NET 8.0  | Serialize,System.Text.Json,JSONArray                   |  15.44 ms | 0.261 ms | 0.231 ms |  0.24 |    0.00 |  750.0000 |  734.3750 |  734.3750 |  7540511 B |       0.411 |
| BenchmarksSerialize                  | &#39;02. JSON array Serialize (non-streaming)&#39;              | .NET 9.0  | .NET 9.0  | Serialize,System.Text.Json,JSONArray                   |  14.04 ms | 0.234 ms | 0.183 ms |  0.22 |    0.00 |  734.3750 |  718.7500 |  718.7500 |  7540761 B |       0.411 |
| BenchmarksSerialize                  | &#39;04. SerializeAsync (JSON array)&#39;                       | .NET 9.0  | .NET 9.0  | Serialize,System.Text.Json,JSONArray                   |  14.37 ms | 0.248 ms | 0.232 ms |  0.23 |    0.00 |  750.0000 |  734.3750 |  734.3750 |  7540776 B |       0.411 |

## Format comparison

Juner.Sequence supports three streaming JSON formats:

- **NDJSON** (`application/x-ndjson`)  
One JSON object per line. The most widely used streaming JSON format.

- **JSON Lines** (`application/jsonl`)  
Semantically identical to NDJSON. Different MIME type and file extension, but same framing rules.

- **JSON Sequence** (`application/json-seq`, RFC 7464)  
Each JSON value is framed using the ASCII Record Separator (0x1E) followed by a newline.  
Designed for robust streaming over transports where newline-delimited framing is ambiguous.

### Why compare these formats?

Although all three formats represent “streaming JSON”, their **framing rules** differ:

| Format        | Framing rule                          | Notes |
|---------------|----------------------------------------|-------|
| NDJSON        | `<json>\n`                             | Most common; simple and efficient |
| JSON Lines    | `<json>\n`                             | Same as NDJSON; different MIME/extension |
| JSON Sequence | `0x1E <json> \n`                       | RFC 7464 framing; slightly higher overhead |

These differences affect:

- **Serialization cost** (extra framing bytes)
- **Deserialization cost** (frame detection)
- **Memory usage** (buffering behavior)
- **First-byte latency** (especially for JSON Sequence)

### What this benchmark measures

This benchmark compares:

- **Serialize**  
Stream / PipeWriter performance for each format

- **DeserializeAsyncEnumerable**  
Streaming deserialization performance for each format  
(NDJSON / JSON Lines / JSON Sequence / JSON array via STJ)

- **Deserialize (non-streaming)**  
Baseline JSON array deserialization for reference

Each benchmark group has its own **Baseline**, so `Ratio` values are meaningful **within the group**.

### Key observations

- **NDJSON / JSON Lines**  
Performance is nearly identical, as expected.  
Both use newline-delimited framing.

- **JSON Sequence**  
Slightly higher cost due to RFC 7464 framing (`0x1E` prefix),  
but still comparable to NDJSON in both throughput and memory usage.

- **System.Text.Json (JSON array)**  
Fastest in raw throughput but **non‑streaming** and requires full buffering.

This section helps clarify how Juner.Sequence behaves across different streaming JSON formats and why these formats matter in real-world streaming scenarios.

## Reproduction

```bash
dotnet run -f net10.0 -c Release -- -r net7.0 net8.0 net9.0 net10.0 --launchCount 1 --memory
```

BenchmarkDotNet builds separate executables for each target runtime.

---

## Notes

This benchmark shows **relative performance**, not absolute throughput.
