# Juner.Http.Sequence

HTTP streaming for record-oriented JSON formats in .NET.

It enables end-to-end streaming from HTTP transport to application code,
without buffering the entire payload.

This allows true streaming pipelines over HTTP with minimal allocations.

`Juner.Http.Sequence` integrates `Juner.Sequence` with `HttpContent` and `HttpRequestMessage`,
providing end‑to‑end streaming support for formats such as:

- **NDJSON** (`application/x-ndjson`)
- **JSON Lines** (`application/jsonl`)
- **JSON Text Sequences** (RFC 7464, `application/json-seq`)

These formats represent a sequence of independent JSON values,
allowing **incremental processing** without loading the entire payload.

---

## Installation

```bash
dotnet add package Juner.Http.Sequence
```

---

## Quick Start

### JsonSerializerContext (AOT‑safe)

```csharp
[JsonSerializable(typeof(MyType))]
public partial class MyJsonContext : JsonSerializerContext { }
```

---

## Reading (streaming)

Assuming you obtained an `HttpContent` from `HttpResponseMessage`:

### Core API (AOT‑safe)

```csharp
await foreach (var item in content.ReadSequenceEnumerable(
    MyJsonContext.Default.MyType,
    SequenceSerializerOptions.JsonLines,
    cancellationToken))
{
    Console.WriteLine(item);
}
```

### Format‑specific shortcuts

```csharp
await foreach (var item in content.ReadJsonLinesAsyncEnumerable(
    MyJsonContext.Default.MyType))
{
    ...
}

await foreach (var item in content.ReadJsonSequenceAsyncEnumerable(
    MyJsonContext.Default.MyType))
{
    ...
}
```

### Notes

- Uses `PipeReader.Create(stream)` internally
- Throws `ArgumentException` if `options.IsInvalid`
- Fully streaming: no buffering of the entire HTTP payload
- Backpressure is naturally handled via `IAsyncEnumerable`

---

## Writing (streaming)

### Core API (AOT‑safe)

```csharp
var request = new HttpRequestMessage(HttpMethod.Post, url)
    .WithSequenceContent(
        source,
        MyJsonContext.Default.MyType,
        SequenceSerializerOptions.JsonLines,
        "application/jsonl");
```

The content type should match the selected sequence format.

### Format‑specific shortcuts

```csharp
request.WithJsonLinesContent(source, MyJsonContext.Default.MyType);
request.WithJsonSequenceContent(source, MyJsonContext.Default.MyType);
request.WithNdJsonContent(source, MyJsonContext.Default.MyType);
```

### Notes

- Uses `PipeWriter.Create(stream)` internally
- Does not compute content length (`TryComputeLength` returns false)
- Streaming is performed record‑by‑record
- Suitable for chunked transfer encoding

---

## AOT‑Friendly Design

All AOT‑safe APIs require:

```csharp
JsonTypeInfo<T>
```

This avoids runtime reflection and ensures compatibility with Native AOT.

---

# Optional Extensions

## JsonSerializerOptions Support — *not guaranteed AOT‑safe*

### Reading

```csharp
await foreach (var item in content.ReadSequenceEnumerable(
    jsonSerializerOptions,
    SequenceSerializerOptions.JsonLines))
{
    ...
}
```

### Writing

```csharp
request.WithSequenceContent(
    source,
    jsonSerializerOptions,
    SequenceSerializerOptions.JsonLines,
    "application/jsonl");
```

### Notes

- Uses `JsonSerializerOptions.TypeInfoResolver`
- Throws if metadata for `T` is not found
- Not AOT‑safe
- On .NET 7 or earlier, `DefaultJsonTypeInfoResolver` is assigned automatically if missing

---

## JsonSerializerOptions.Default Support — *explicitly not AOT‑safe*

### Reading

```csharp
await foreach (var item in content.ReadJsonLinesAsyncEnumerable())
{
    ...
}
```

### Writing

```csharp
request.WithJsonLinesContent(source);
```

These APIs:

- Use `JsonSerializerOptions.Default`
- Are annotated with:
  - `RequiresUnreferencedCode`
  - `RequiresDynamicCode`
- Are convenience wrappers around the JsonSerializerOptions APIs
- Not AOT‑safe

---

## non‑generic JsonTypeInfo Support — *advanced use only*

### Reading

```csharp
await foreach (var item in content.ReadSequenceEnumerable(
    (JsonTypeInfo)jsonTypeInfo,
    SequenceSerializerOptions.JsonLines))
{
    ...
}
```

### Writing

```csharp
request.WithSequenceContent(
    source,
    (JsonTypeInfo)jsonTypeInfo,
    SequenceSerializerOptions.JsonLines,
    "application/jsonl");
```

### Notes

- Throws if the provided `JsonTypeInfo` does not match `T`
- Not AOT‑safe
- Provided only for advanced scenarios

---

## Supported Formats

The following formats are supported:

| Format | Content-Type | Notes |
|--------|--------------|-------|
| NDJSON | application/x-ndjson | newline‑delimited |
| JSON Lines | application/jsonl | equivalent to NDJSON |
| JSON Sequence | application/json-seq | RFC 7464 (RS‑delimited) |

---

## About JSON Array (`application/json`)

JSON arrays are already well supported by standard JSON APIs.

Juner.Http.Sequence focuses on record-oriented streaming formats,
and does not provide support for JSON arrays.

If you need to read or write JSON arrays, use:

```csharp
HttpContent.ReadFromJsonAsync<T[]>()
```

or the standard `JsonSerializer`.

---

## Architecture

The library is built as a thin HTTP layer on top of Juner.Sequence:

```mermaid
graph TD;
    A[Juner.Sequence<br/>Core]
    B[Juner.Http.Sequence]

    A --> B

    B --> C[HttpContent Extensions]
    B --> D[HttpRequestMessage Extensions]

    B --> E[JsonSerializerOptions Extensions<br/>(not AOT‑safe)]
    B --> F[JsonSerializerOptions.Default Extensions<br/>(explicitly not AOT‑safe)]
    B --> G[JsonTypeInfo (non‑generic)<br/>Advanced]
```

---

## When to Use

- Consuming large streaming JSON responses
- Real‑time event streams over HTTP
- High‑performance pipelines
- Native AOT applications
- Avoiding buffering entire HTTP payloads

## When NOT to Use

- Small payloads → `HttpContent.ReadFromJsonAsync` is simpler
- JSON arrays → use standard JSON APIs
- Native AOT scenarios → avoid `JsonSerializerOptions.Default` (not AOT‑safe)

---

## License

MIT
