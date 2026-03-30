# Juner.AspNetCore.Sequence

Streaming support for record‑oriented JSON formats (NDJSON, JSON Lines, JSON Sequence) in ASP.NET Core MVC and Minimal API.

This package integrates **Juner.Sequence** with ASP.NET Core, providing:

- Streaming **input** via `SequenceInputFormatter` and `Sequence<T>`
- Streaming **output** via `JsonSequenceResult`, `JsonLineResult`, `NdJsonResult`, and `SequenceResult`
- Minimal API integration with natural syntax
- MVC integration through formatters and metadata
- OpenAPI (.NET 10+) support for streaming schemas
- Content negotiation for streaming formats

It enables true end‑to‑end streaming pipelines in ASP.NET Core without buffering entire payloads.

---

## Installation

```bash
dotnet add package Juner.AspNetCore.Sequence
```

---

# Quick Start

## Minimal API — Streaming JSON Output

```csharp
app.MapGet("/events", () =>
    TypedResults.JsonSequence(GetEvents()));

static async IAsyncEnumerable<Event> GetEvents()
{
    while (true)
    {
        yield return new Event { Message = "tick", Time = DateTime.UtcNow };
        await Task.Delay(1000);
    }
}
```

## Minimal API — Streaming JSON Input

```csharp
app.MapPost("/upload", async (Sequence<MyType> items) =>
{
    await foreach (var item in items)
        Console.WriteLine(item);
});
```

`Sequence<T>` is an ASP.NET Core–native type that binds streaming JSON inputs.

---

# Supported Streaming Formats

| Format | Content-Type | Notes |
|--------|--------------|-------|
| JSON Sequence | `application/json-seq` | RFC 7464 (RS‑delimited) |
| NDJSON | `application/x-ndjson` | newline‑delimited |
| JSON Lines | `application/jsonl` | equivalent to NDJSON |

---

# Streaming Output

ASP.NET Core actions can return the following types as streaming JSON:

| Return Type | Streaming? | Notes |
|-------------|------------|-------|
| `IAsyncEnumerable<T>` | ✔ | ideal for streaming |
| `ChannelReader<T>` | ✔ | backpressure‑friendly |
| `IEnumerable<T>` | △ | buffered |
| `List<T>` | △ | buffered |
| `T[]` | △ | buffered |
| `Sequence<T>` | ✔ | ASP.NET Core–native streaming |

### Minimal API Result Types

```csharp
return TypedResults.JsonSequence(values);
return TypedResults.JsonLine(values);
return TypedResults.NdJson(values);
return TypedResults.Sequence(values); // content negotiation
```

### MVC OutputFormatter

Streaming is enabled when the client sends:

- `Accept: application/json-seq`
- `Accept: application/x-ndjson`
- `Accept: application/jsonl`

---

# Streaming Input

ASP.NET Core actions can accept the following types as streaming input:

| Parameter Type | Streaming? |
|----------------|------------|
| `Sequence<T>` | ✔ |
| `IAsyncEnumerable<T>` | ✔ |
| `ChannelReader<T>` | ✔ |
| `IEnumerable<T>` | △ (buffered) |
| `List<T>` | △ |
| `T[]` | △ |

### Minimal API Example

```csharp
app.MapPost("/items", async (Sequence<Item> items) =>
{
    await foreach (var item in items)
        Console.WriteLine(item);
});
```

### MVC InputFormatter

Streaming is enabled for:

- `application/json-seq`
- `application/x-ndjson`
- `application/jsonl`

---

# Content Negotiation

`SequenceResult<T>` automatically selects the best output format based on the `Accept` header:

| Accept | Output |
|--------|--------|
| `application/json-seq` | JSON Sequence |
| `application/x-ndjson` | NDJSON |
| `application/jsonl` | JSON Lines |
| `application/json` | JSON array (non‑streaming) |

```csharp
return TypedResults.Sequence(values);
```

---

# JSON Array (`application/json`)

JSON arrays are **not** streaming formats.

`SequenceResult<T>` can return JSON arrays, but they are **fully buffered**.

For true streaming, use:

- `JsonSequenceResult<T>`
- `JsonLineResult<T>`
- `NdJsonResult<T>`

---

# OpenAPI Integration (.NET 10+)

Enable OpenAPI support:

```csharp
services.AddSequenceOpenApi();
```

Streaming endpoints are annotated with:

- `x-streaming: true`
- `x-itemSchema: { ... }`
- Correct content types per format

Both request and response schemas are generated accurately.

---

# Architecture

```mermaid
graph TD;
    A[Juner.Sequence<br/>Core]
    B[Juner.Http.Sequence]
    C[Juner.AspNetCore.Sequence]

    A --> B
    B --> C

    C --> D[InputFormatter<br/>SequenceInputFormatter]
    C --> E[OutputFormatter<br/>JsonSequence / JsonLine / NdJson]
    C --> F[Result Types<br/>JsonSequenceResult / JsonLineResult / NdJsonResult / SequenceResult]
    C --> G[Sequence<T><br/>Minimal API Integration]
    C --> H[OpenAPI (.NET 10+)]
```

---

# AOT Considerations

ASP.NET Core formatters, metadata, and result types rely on:

- dynamic code generation
- reflection
- `JsonSerializerOptions` and `TypeInfoResolver`

Therefore, **this package is not AOT‑safe**.

Native AOT applications cannot use this library.

---

# Samples

This repository includes two complete samples:

- **Minimal API JSON Sequence Streaming Sample**  
- **MVC JSON Sequence Streaming Sample**

Both demonstrate:

- Streaming output (`JsonSequenceResult`)
- Streaming input (`Sequence<T>`)
- Bidirectional streaming using `fetch()` with `duplex: 'half'`
- Browser‑side JSON Sequence parsing (`json-seq-stream`)
- OpenAPI (.NET 10+) integration

### Minimal API Sample

Located at:

```
../samples/AspNetCore.Sequence/MinimalApiJsonSequenceStreamingSample.cs
```

### MVC Sample

Located at:

```
../samples/AspNetCore.Sequence/MvcJsonSequenceStreamingSample.cs
```

---

# License

MIT
