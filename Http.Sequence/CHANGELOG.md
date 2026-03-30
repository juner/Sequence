# Changelog — Juner.Http.Sequence

## [1.0.0] — 2026-03-31

### Added

- Initial release of HttpClient integration for Juner.Sequence.
- `HttpContent` extensions for streaming JSON:
  - `WithNdJsonContent`
  - `WithJsonLinesContent`
  - `WithJsonSequenceContent`
- Streaming deserialization helpers:
  - `ReadJsonLinesAsyncEnumerable<T>`
  - `ReadJsonSequenceAsyncEnumerable<T>`
- Automatic Content-Type assignment for streaming formats.
- AOT‑friendly API using `JsonTypeInfo<T>`.

### Notes

- No dependency on `Juner.AspNetCore.Sequence`.
- JSON Array (`application/json`) is not treated as a streaming format.
