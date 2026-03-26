using Juner.Sequence;
using Juner.Sequence.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Channels;

#if NET9_0_OR_GREATER
using System.IO.Pipelines;
#endif

namespace Juner.AspNetCore.Sequence.Internals;

internal static class InternalFormatWriter
{
    static IDictionary<Type, EnumerableType>? _targetInterface;
    static IDictionary<Type, EnumerableType> TargetInterfaces => _targetInterface ??= new Dictionary<Type, EnumerableType>()
    {
        {typeof(IAsyncEnumerable<>), EnumerableType.AsyncEnumerable },
        {typeof(IEnumerable<>), EnumerableType.Enumerable },
        {typeof(ChannelReader<>), EnumerableType.ChannelReader },
        {typeof(Http.Sequence<>), EnumerableType.Sequence },
    }.AsReadOnly();
    public static bool TryGetOutputMode([NotNullWhen(true)] Type? objectType, [NotNullWhen(true)] out EnumerableType outputType, [NotNullWhen(true)] out Type type)
    {
        outputType = default;
        type = default!;
        // 型なしは無視する
        if (objectType is null) return false;
        // 文字列は除外
        if (objectType == typeof(string)) return false;
        var interfaces = objectType switch
        {
            { IsInterface: true } => [objectType, .. objectType.GetInterfaces()],
            _ => objectType.GetInterfaces().Where(v => v.IsGenericType),
        };
        var find = false;
        foreach (var i in interfaces)
        {
            find = TargetInterfaces.TryGetValue(i.GetGenericTypeDefinition(), out outputType);
            if (find)
            {
                type = i.GetGenericArguments()[0];
                break;
            }
        }
        return find;
    }

    static JsonTypeInfo GetJsonTypeInfo(JsonSerializerOptions serializerOptions, Type type) => serializerOptions.GetTypeInfo(type);
    public static Task WriteResponseBodyAsync(
        Type? objectType,
        object? @object,
        HttpContext httpContext,
        JsonSerializerOptions serializerOptions,
        Encoding selectedEncoding,
        ISequenceSerializerWriteOptions options,
        CancellationToken cancellationToken)
    {
        if (!TryGetOutputMode(objectType, out _, out var type))
            throw new InvalidOperationException();
        return WriteAsync(
            objectType,
            @object,
            httpContext,
            GetJsonTypeInfo(serializerOptions, type),
            selectedEncoding,
            options,
            cancellationToken);
    }

    public static Task WriteAsync<Enumerable, T>(
        Enumerable? @object,
        HttpContext httpContext,
        JsonTypeInfo jsonTypeInfo,
        Encoding SelectedEncoding,
        ISequenceSerializerWriteOptions options,
        CancellationToken cancellationToken)
        where T : notnull
    {
        if (!TryGetOutputMode(typeof(Enumerable), out var OutputType, out var type))
            throw new InvalidOperationException($"not support output type ");
        var jsonTypeInfo2 = (JsonTypeInfo<T>)jsonTypeInfo;
        var newValues = OutputType switch
        {
            EnumerableType.AsyncEnumerable or EnumerableType.Sequence => @object as IAsyncEnumerable<T>,
            EnumerableType.Enumerable or EnumerableType.Array or EnumerableType.List => ToAsyncEnumerable(@object as IEnumerable<T>, cancellationToken),
            EnumerableType.ChannelReader => ToAsyncEnumerable(@object as ChannelReader<T>, cancellationToken),
            _ => throw new NotImplementedException($"not support pattern {@object?.GetType().Name ?? "null"} and {OutputType}"),
        };
        if (newValues is null)
            return Task.CompletedTask;
        if (SelectedEncoding == null || SelectedEncoding == Encoding.UTF8)
            return SequenceSerializer.SerializeAsync(httpContext.Response.BodyWriter, newValues, jsonTypeInfo2, options, cancellationToken);
        else
            return SequenceSerializerEncodeExntensions.SerializeAsync(httpContext.Response.BodyWriter, newValues, jsonTypeInfo2, options, SelectedEncoding, cancellationToken);
    }
    static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(ChannelReader<T>? values, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (values is null) yield break;
        while (await values.WaitToReadAsync(cancellationToken))
            if (values.TryRead(out var item))
                yield return item;
    }
    static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T>? values, [EnumeratorCancellation] CancellationToken _)
    {
        if (values is null) yield break;
        foreach (var item in values)
            yield return item;
    }

    static readonly ConcurrentDictionary<Type, Delegate> cache = new();

    static Task WriteAsync(
        Type objectType,
        object? @object,
        HttpContext httpContext,
        JsonTypeInfo jsonTypeInfo,
        Encoding selectedEncoding,
        ISequenceSerializerWriteOptions options,
        CancellationToken cancellationToken)
    {
        var del = cache.GetOrAdd(objectType, CreateDelegate);

        var func =
            (Func<
                object?,
                HttpContext,
                JsonTypeInfo,
                Encoding,
                ISequenceSerializerWriteOptions,
                CancellationToken,
                Task>)del;

        return func(
            @object,
            httpContext,
            jsonTypeInfo,
            selectedEncoding,
            options,
            cancellationToken);
    }

    static Delegate CreateDelegate(Type objectType)
    {
        if (!TryGetOutputMode(objectType, out _, out var type))
            throw new InvalidOperationException($"{objectType} not found elementType");
        var method =
            typeof(InternalFormatWriter)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(static m => m is
            {
                Name: nameof(WriteAsync),
                IsGenericMethodDefinition: true
            } && m.GetParameters() is { Length: 6 })
            .MakeGenericMethod(objectType, type);

        // parameters
        var pObj = Expression.Parameter(typeof(object), "object");
        var pHttp = Expression.Parameter(typeof(HttpContext), "httpContext");
        var pJsonTypeInfo = Expression.Parameter(typeof(JsonTypeInfo), "jsonTypeInfo");
        var pEncoding = Expression.Parameter(typeof(Encoding), "selectedEncoding");
        var pOptions = Expression.Parameter(typeof(ISequenceSerializerWriteOptions), "options");
        var pCancel = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        // object → T
        var castObj = Expression.Convert(pObj, objectType);

        // JsonTypeInfo → JsonTypeInfo<T>
        var jsonTypeInfoT = typeof(JsonTypeInfo<>).MakeGenericType(type);
        var castJsonTypeInfo = Expression.Convert(pJsonTypeInfo, jsonTypeInfoT);

        var call = Expression.Call(
            method,
            castObj,
            pHttp,
            castJsonTypeInfo,
            pEncoding,
            pOptions,
            pCancel);

        var lambda =
            Expression.Lambda<
                Func<
                    object?,
                    HttpContext,
                    JsonTypeInfo,
                    Encoding,
                    ISequenceSerializerWriteOptions,
                    CancellationToken,
                    Task>>
            (
                call,
                pObj,
                pHttp,
                pJsonTypeInfo,
                pEncoding,
                pOptions,
                pCancel
            );

        return lambda.Compile();
    }
}