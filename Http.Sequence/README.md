# Juner.Http.Sequence

Streaming JSON sequence support for `HttpClient` and `HttpContent`.

This library provides seamless integration between `Juner.Sequence` and HTTP APIs, enabling efficient streaming of JSON data using `IAsyncEnumerable<T>`.

---

## 📦 Package

- `Juner.Http.Sequence`

---

## 🚀 Features

- 🌐 `HttpClient` integration for streaming JSON
- 🔄 `IAsyncEnumerable<T>` support for request and response
- 🧩 Supports:
  - NDJSON (`application/x-ndjson`)
  - JSON Lines (`application/jsonl`)
  - JSON Sequence (`application/json-seq`)
- 🛡️ AOT-friendly via `JsonTypeInfo<T>`
- ⚡ Minimal overhead, fully streaming

---

## ✨ Quick Example

### Send streaming request (NDJSON)

```csharp
var request = new HttpRequestMessage(HttpMethod.Post, url)
    .WithNdJsonContent(source, MyJsonContext.Default.MyType);

var response = await httpClient.SendAsync(request);
```

---

### Receive streaming response

```csharp
await foreach (var item in response.Content.ReadJsonLinesAsyncEnumerable<MyType>(
    MyJsonContext.Default.MyType))
{
    Console.WriteLine(item);
}
```

---

## 🧠 API Design

### ✅ AOT-safe (recommended)

```csharp
JsonTypeInfo<T>
```

- No reflection
- Fully compatible with Native AOT
- Best performance

---

### ⚠️ Convenience APIs

```csharp
JsonSerializerOptions.Default
```

- Easier to use
- Uses default metadata resolution (`TypeInfoResolver`)
- May rely on reflection
- Not guaranteed to be AOT-safe

👉 Prefer `JsonTypeInfo<T>` for AOT scenarios

---

## 🔧 Writing (Request)

```csharp
request.WithSequenceContent(
    source,
    MyJsonContext.Default.MyType,
    SequenceSerializerOptions.JsonLines,
    "application/jsonl");
```

### Shortcuts

```csharp
request.WithJsonSequenceContent(source, typeInfo);
request.WithJsonLinesContent(source, typeInfo);
request.WithNdJsonContent(source, typeInfo);
```

---

## 🔧 Reading (Response)

```csharp
await foreach (var item in response.Content.ReadSequenceEnumerable<T>(
    typeInfo,
    SequenceSerializerOptions.JsonLines))
{
    // ...
}
```

### Shortcuts

```csharp
response.Content.ReadJsonSequenceAsyncEnumerable<T>(typeInfo);
response.Content.ReadJsonLinesAsyncEnumerable<T>(typeInfo);
```

---

## 🧩 Supported Formats

| Format | Content-Type |
|-------|-------------|
| NDJSON | `application/x-ndjson` |
| JSON Lines | `application/jsonl` |
| JSON Sequence | `application/json-seq` |

---

## 🔗 Relationship

```
Juner.Sequence
    ↓
Juner.Http.Sequence
```

- Depends on `Juner.Sequence`
- Adds HTTP integration layer
- No ASP.NET Core dependency

---

## 📌 When to use

Use this package when:

- You need to stream large JSON datasets over HTTP
- You want `HttpClient` to work with `IAsyncEnumerable<T>`
- You are working with NDJSON / JSON Lines APIs
- You want AOT-friendly serialization over HTTP

---

## 📄 License

MIT

---

## 🔗 Links

- GitHub: https://github.com/juner/Sequence
- NuGet: https://www.nuget.org/packages/Juner.Http.Sequence

---

## 🙌 Contributions

Issues and PRs are welcome!
