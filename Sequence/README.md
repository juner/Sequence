# Juner.Sequence

A high-performance, AOT-friendly JSON sequence serializer for .NET.

`Juner.Sequence` provides streaming serialization and deserialization for sequence-based JSON formats such as:

- JSON Lines (`.jsonl`)
- JSON Text Sequences (RFC 7464, RS-delimited)

It is designed with **System.IO.Pipelines** and **System.Text.Json** in mind, focusing on:

- 🚀 High performance (minimal allocations)
- 🔒 AOT compatibility (no reflection by default)
- 🔄 True streaming via `IAsyncEnumerable<T>`

---

## ✨ Features

- Fully streaming (no full buffering required)
- `JsonTypeInfo<T>`-based (AOT safe)
- Supports JSON Lines and JSON Sequence formats
- Works directly with `PipeReader` / `PipeWriter`
- Optional extensions for:
  - Encoding support
  - `JsonSerializerOptions` compatibility

---

## 📦 Installation

```bash
dotnet add package Juner.Sequence
```

---

## 🚀 Quick Start

### Serialize

```csharp
await SequenceSerializer.SerializeAsync(
    writer,
    asyncEnumerable,
    jsonTypeInfo,
    SequenceSerializerOptions.JsonLines,
    cancellationToken);
```

---

### Deserialize

```csharp
await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
    reader,
    jsonTypeInfo,
    SequenceSerializerOptions.JsonLines,
    cancellationToken))
{
    Console.WriteLine(item);
}
```

---

## 🔑 AOT-Friendly Design

This library is built around:

```csharp
JsonTypeInfo<T>
```

instead of `JsonSerializerOptions`.

### Why?

- No runtime reflection
- Works with Native AOT
- Better performance and predictability

---

## ⚠️ Optional: JsonSerializerOptions Support

You can opt-in to `JsonSerializerOptions` support:

```csharp
using Juner.Sequence.Extensions.Json;
```

Example:

```csharp
await SequenceSerializer.SerializeAsync(
    writer,
    asyncEnumerable,
    jsonSerializerOptions,
    SequenceSerializerOptions.JsonLines);
```

> ⚠️ These APIs are **not AOT-safe** and may use reflection.

---

## 🌐 Encoding Support

By default, the library operates in UTF-8.

To use other encodings:

```csharp
using Juner.Sequence.Extensions;

await SequenceSerializer.SerializeAsync(
    writer,
    asyncEnumerable,
    jsonTypeInfo,
    SequenceSerializerOptions.JsonLines,
    Encoding.UTF8);
```

Internally, this uses a transcoding stream.

---

## 📚 Supported Formats

### JSON Lines

```json
{"id":1}
{"id":2}
```

---

### JSON Text Sequence (RFC 7464)

```
RS {"id":1}
RS {"id":2}
```

---

## 🧱 Architecture

```
Juner.Sequence
 ├ Core (AOT-safe)
 │   └ JsonTypeInfo<T> APIs
 │
 ├ Extensions
 │   └ Encoding support
 │
 └ Extensions.Json
     └ JsonSerializerOptions support (⚠ not AOT-safe)
```

---

## 🧪 Example with PipeReader

```csharp
var reader = PipeReader.Create(stream);

await foreach (var item in SequenceSerializer.DeserializeAsyncEnumerable(
    reader,
    jsonTypeInfo,
    SequenceSerializerOptions.JsonLines))
{
    // process item
}
```

---

## 📌 When to Use

Use this library when:

- Processing large JSON streams
- Building high-performance APIs
- Targeting Native AOT
- Working with pipelines or streaming systems

---

## ⚠️ When NOT to Use

- Small payloads → use `JsonSerializer`
- Reflection-heavy scenarios → use `JsonSerializerOptions` directly

---

## 📄 License

MIT License

---

## 🙌 Contributing

Contributions are welcome!  
Feel free to open issues or pull requests.

---

## 💡 Notes

- Prefer `JsonTypeInfo<T>` for best performance and AOT safety
- Use extensions only when necessary
- Keep streaming — avoid buffering