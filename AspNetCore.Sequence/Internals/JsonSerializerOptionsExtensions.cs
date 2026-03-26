using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Juner.AspNetCore.Sequence.Internals;

internal static class JsonSerializerOptionsExtensions
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonTypeInfo<T> GetTypeInfo<T>(this JsonSerializerOptions options) =>
        (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
}