# Juner.Sequence

[![NuGet](https://img.shields.io/nuget/v/Juner.Sequence.svg)](https://www.nuget.org/packages/Juner.Sequence/)
[![NuGet](https://img.shields.io/nuget/v/Juner.Http.Sequence.svg)](https://www.nuget.org/packages/Juner.Http.Sequence/)
[![NuGet](https://img.shields.io/nuget/v/Juner.AspNetCore.Sequence.svg)](https://www.nuget.org/packages/Juner.AspNetCore.Sequence/)
[![.NET Test](https://github.com/juner/Sequence/actions/workflows/test.yml/badge.svg)](https://github.com/juner/Sequence/actions/workflows/test.yml)

High-performance streaming JSON serialization for .NET using `IAsyncEnumerable<T>`.  
Supports modern streaming-friendly JSON formats:

- **NDJSON** (`application/x-ndjson`)
- **JSON Lines** (`application/jsonl`)
- **JSON Sequence** (`application/json-seq`)

Built on top of `System.Text.Json` with an **AOT-friendly**, **zero-allocation**, and **layered** design.

---

## Packages

| Package | Description |
|--------|-------------|
| **Juner.Sequence** | Core streaming serializer (no HTTP dependency) |
| **Juner.Http.Sequence** | `HttpClient` / `HttpContent` integration |
| **Juner.AspNetCore.Sequence** | ASP.NET Core input/output streaming support |

---

## Features

- ⚡ High-performance streaming JSON  
- 🔄 Full `IAsyncEnumerable<T>` support  
- 🧩 Multiple streaming formats (NDJSON / JSON Lines / JSON Sequence)  
- 🛡️ AOT‑friendly (`JsonTypeInfo<T>`‑based API; no reflection)  
- 🧼 Clean and layered architecture  
- 🌐 HTTP and ASP.NET Core integration

---

## Architecture

```mermaid
graph TD;
    A[Juner.Sequence<br/>Core] --> B[Juner.Http.Sequence<br/>HTTP Integration];
    A --> C[Juner.AspNetCore.Sequence<br/>ASP.NET Core];
```

---

## Quick Start

### Serialize (NDJSON)

```csharp
await SequenceSerializer.SerializeAsync(
    stream,
    source,
    MyJsonContext.Default.MyType,
    SequenceSerializerOptions.JsonLines,
    cancellationToken);
```

### Deserialize (streaming)

```csharp
await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
    stream,
    MyJsonContext.Default.MyType,
    SequenceSerializerOptions.JsonLines,
    cancellationToken))
{
    Console.WriteLine(item);
}
```

---

## HttpClient Integration

```csharp
var request = new HttpRequestMessage(HttpMethod.Post, url)
    .WithNdJsonContent(source, MyJsonContext.Default.MyType);

var response = await httpClient.SendAsync(request);

await foreach (var item in response.Content.ReadJsonLinesAsyncEnumerable<MyType>(
    MyJsonContext.Default.MyType))
{
    Console.WriteLine(item);
}
```

---

## API Design

### AOT-safe API (Recommended)

- No reflection  
- Fully compatible with Native AOT  
- Uses `JsonTypeInfo<T>`

### Convenience API

- Based on `JsonSerializerOptions`  
- May require reflection  
- Not guaranteed to be AOT-safe  

---

## Supported Streaming Formats

| Format | Content-Type | Option |
|--------|--------------|--------|
| NDJSON | application/x-ndjson | `JsonLines` |
| JSON Lines | application/jsonl | `JsonLines` |
| JSON Sequence | application/json-seq | `JsonSequence` |

*(JSON Array intentionally omitted — see Note below.)*

---

## Notes on JSON Array Support

> **Note**  
> `application/json` (JSON arrays) is **not** a dedicated streaming format of this library.  
> However, `Juner.AspNetCore.Sequence` accepts JSON arrays **only**:
>
> - when binding to `Sequence<T>` (Minimal API model binding), or  
> - when using `SequenceResults.Sequence(source)`  
>
> This is provided **for convenience only**, and is not part of the core streaming format set.

---

## License

MIT License  
See the `LICENSE` file for details.

---

## Links

- Repository: https://github.com/juner/Sequence
