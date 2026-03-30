# Changelog — Juner.Sequence

## [1.0.0] — 2026-03-31

### Added

- Initial stable release of the core streaming JSON serializer.
- High‑performance serialization for:
  - NDJSON (`application/x-ndjson`)
  - JSON Lines (`application/jsonl`)
  - JSON Sequence (`application/json-seq`, RFC 7464)
- Fully streaming deserialization via `DeserializeAsyncEnumerable`.
- Fully streaming serialization via `SerializeAsync`.
- Zero‑allocation, AOT‑friendly API using `JsonTypeInfo<T>`.
- Convenience API using `JsonSerializerOptions`.
- Pluggable `ISequenceSerializerWriteOptions` and `ISequenceSerializerReadOptions`.
- Unified `SequenceSerializerOptions` presets (`JsonLines`, `JsonSequence`, `Default`).
- Strict separation from HTTP concerns (no HttpClient / ASP.NET Core dependencies).

### Notes

- JSON Array (`application/json`) is intentionally **not supported** as a streaming format.
