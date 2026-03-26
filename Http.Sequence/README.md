# Juner.Http.Sequence

HTTP streaming extensions for `Juner.Sequence`.

This library provides a minimal bridge between `HttpClient` and `Juner.Sequence`, enabling:

- Streaming **read** from `HttpContent`
- Streaming **write** via `HttpRequestMessage`
- Support for JSON Lines and JSON Sequence formats

---

## ✨ Features

- 📥 Read `IAsyncEnumerable<T>` directly from `HttpContent`
- 📤 Send `IAsyncEnumerable<T>` as HTTP request content
- 🚀 Zero-buffer streaming
- 🔒 AOT-friendly (via `JsonTypeInfo<T>`)
- 🧩 Minimal and dependency-light

---

## 📦 Installation

```bash
dotnet add package Juner.Http.Sequence
```

---

## 🚀 Quick Start

---

### 📥 Read streaming response

```csharp
using var response = await httpClient.GetAsync(
    "/data",
    HttpCompletionOption.ResponseHeadersRead);

await foreach (var item in response.Content.ReadJsonLinesAsyncEnumerable(
    AppJsonContext.Default.TestData))
{
    Console.WriteLine(item);
}
```

---

### 📤 Send streaming request

```csharp
var request = new HttpRequestMessage(HttpMethod.Post, "/data")
    .WithJsonLinesContent(
        GetAsyncEnumerable(),
        AppJsonContext.Default.TestData);

await httpClient.SendAsync(request);
```

---

## 📥 Reading APIs

### Generic

```csharp
content.ReadSequenceEnumerable<T>(typeInfo, options)
```

### JSON Sequence (RFC 7464)

```csharp
content.ReadJsonSequenceAsyncEnumerable<T>(typeInfo)
```

### JSON Lines

```csharp
content.ReadJsonLinesAsyncEnumerable<T>(typeInfo)
```

---

## 📤 Writing APIs

### Generic

```csharp
request.WithSequenceContent(
    source,
    typeInfo,
    options,
    contentType)
```

### JSON Sequence

```csharp
request.WithJsonSequenceContent(source, typeInfo)
```

### JSON Lines

```csharp
request.WithJsonLinesContent(source, typeInfo)
```

### NDJSON

```csharp
request.WithNdJsonContent(source, typeInfo)
```

---

## 📡 Content Types

| Format | Content-Type |
|--------|-------------|
| JSON Sequence | `application/json-seq` |
| JSON Lines | `application/jsonl` |
| NDJSON | `application/x-ndjson` |

---

## 🧱 How It Works

```
HttpContent (Stream)
    ↓
PipeReader
    ↓
Juner.Sequence
    ↓
IAsyncEnumerable<T>
```

```
IAsyncEnumerable<T>
    ↓
Juner.Sequence
    ↓
PipeWriter
    ↓
HttpContent
```

---

## 🔗 Relationship with Juner.Sequence

This library depends on `Juner.Sequence` for serialization.

If you need lower-level control over pipelines or formats,
use `Juner.Sequence` directly.

---

## ⚠️ Notes

- `JsonTypeInfo<T>` is required (AOT-friendly)
- Streaming requires `HttpCompletionOption.ResponseHeadersRead`
- `Content-Length` is not computed (chunked transfer)

---

## 📄 License

MIT License