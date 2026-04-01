# Benchmarks Overview

This repository contains benchmarks for all Juner.* packages.

- **Juner.Sequence**  
  Pure streaming JSON serialization/deserialization performance  
  → [benchmarks/Sequence.Benchmarks/BENCHMARKS.md](./Sequence.Benchmarks/BENCHMARKS.md)

- **Juner.Http.Sequence**  
  HTTP client-side streaming performance  
  (NDJSON vs JSON Lines vs JSON Sequence)  
  → [benchmarks/Http.Sequence.Benchmarks/BENCHMARKS.md](./Http.Sequence.Benchmarks/BENCHMARKS.md)

- **Juner.AspNetCore.Sequence**  
  Minimal API streaming performance  
  (NDJSON vs JSON array (buffered) vs JSON array (IAsyncEnumerable streaming))  
  → [benchmarks/AspNetCore.Sequence.Benchmarks/BENCHMARKS.md](./AspNetCore.Sequence.Benchmarks/BENCHMARKS.md)
