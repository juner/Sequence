# Changelog — Juner.AspNetCore.Sequence

## [1.0.0] — 2026-03-31

### Added

- Initial release of ASP.NET Core integration for streaming JSON.
- Streaming **output** via:
  - `JsonSequenceResult<T>`
  - `JsonLineResult<T>`
  - `NdJsonResult<T>`
  - `SequenceResult<T>` (content negotiation)
- Streaming **input** via:
  - `Sequence<T>` (Minimal API model binding)
  - MVC `SequenceInputFormatter`
- Streaming **output** for MVC via:
  - `JsonSequenceOutputFormatter`
  - `JsonLineOutputFormatter`
  - `NdJsonOutputFormatter`
- Content negotiation for streaming formats (`Accept` header).
- OpenAPI (.NET 10+) support:
  - `x-streaming: true`
  - `x-itemSchema`
  - Per‑format content types
- Minimal API integration:
  - `TypedResults.JsonSequence`
  - `TypedResults.JsonLine`
  - `TypedResults.NdJson`
  - `TypedResults.Sequence`
- MVC integration:
  - `[Consumes]` / `[Produces]` metadata for streaming formats
  - Controller binding for `Sequence<T>`

### Notes

- Minimal API streaming is **AOT‑safe**.
- MVC streaming uses formatters and is **not AOT‑safe** due to ASP.NET Core limitations.
- JSON Array (`application/json`) is accepted/returned only as a **non‑streaming convenience**.
