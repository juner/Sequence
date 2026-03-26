# Juner.Sequence

High-performance streaming JSON serialization for .NET using `IAsyncEnumerable<T>`.

Supports:

- NDJSON (Newline Delimited JSON)
- JSON Lines
- JSON Sequence (`application/json-seq`)
- JSON arrays

Built on top of `System.Text.Json` with **AOT-friendly design** and **zero-allocation streaming** in mind.

---

## 📦 Packages

| Package | Description |
|--------|------------|
| `Juner.Sequence` | Core streaming serializer (no HTTP dependency) |
| `Juner.Http.Sequence` | HttpClient / HttpContent integration |
| `Juner.AspNetCore.Sequence` | ASP.NET Core request/response streaming support |

---

## 🚀 Features

- ⚡ High-performance streaming JSON
- 🔄 Full `IAsyncEnumerable<T>` support
- 🧩 Multiple formats (NDJSON / JSON Lines / JSON Sequence / Array)
- 🛡️ AOT-friendly (`JsonTypeInfo<T>`-based API)
- 🧱 Clean layered architecture
- 🌐 HTTP integration support

---

## ✨ Quick Example

### Serialize (NDJSON)

```csharp
await SequenceSerializer.SerializeAsync(
    stream,
    source,
    MyJsonContext.Default.MyType,
    SequenceSerializerOptions.JsonLines,
    cancellationToken);
```

---

### Deserialize (Streaming)

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

## 🌐 HttpClient Integration

Using `Juner.Http.Sequence`:

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

## 🧠 API Design

This library provides two styles of APIs:

### ✅ AOT-safe (recommended)

```csharp
JsonTypeInfo<T>
```

- No reflection
- Fully compatible with Native AOT

---

### ⚠️ Convenience APIs

```csharp
JsonSerializerOptions.Default
```

- Easier to use
- May require reflection
- Not guaranteed AOT-safe

---

## 🧩 Supported Formats

| Format | Content-Type | Option |
|-------|-------------|--------|
| NDJSON | `application/x-ndjson` | `JsonLines` |
| JSON Lines | `application/jsonl` | `JsonLines` |
| JSON Sequence | `application/json-seq` | `JsonSequence` |

---

## 🏗️ Architecture

```
Juner.Sequence
    ↓
Juner.Http.Sequence
    ↓
Juner.AspNetCore.Sequence
```

- Core is independent from HTTP
- Extensions layer adds integration
- ASP.NET Core layer is optional

---

## 📌 Why Juner.Sequence?

- `System.Text.Json` does not provide streaming sequence formats out-of-the-box
- Existing solutions often:
  - allocate heavily
  - lack AOT support
  - are tightly coupled to frameworks

👉 **Juner.Sequence solves these problems with a clean, composable design**

---

## 📄 License

MIT

---

## 🔗 Links

- GitHub: https://github.com/juner/Sequence
- NuGet:
  - https://www.nuget.org/packages/Juner.Sequence
  - https://www.nuget.org/packages/Juner.Http.Sequence
  - https://www.nuget.org/packages/Juner.AspNetCore.Sequence

---

## 🙌 Contributions

Issues and PRs are welcome!
