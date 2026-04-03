using Juner.AspNetCore.Sequence.Internals;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Diagnostics;
using System.Text.Json;

using Juner.Sequence;

#if !NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif                        

namespace Juner.AspNetCore.Sequence.Http.HttpResults;

[DebuggerDisplay("{Values,nq}")]
public partial class SequenceResult<T> : IResult
{
    static ILogger GetLogger(IServiceProvider provider) => provider.GetService<ILogger<SequenceResult<T>>>() ?? (ILogger)NullLogger.Instance;
    static JsonSerializerOptions GetOptions(IServiceProvider provider, ILogger logger)
    {
        var jsonOptions = provider.GetService<IOptions<JsonOptions>>()?.Value;
        if (jsonOptions is null)
        {
            Log.LogNotHaveJsonOptions(logger);
            jsonOptions = new JsonOptions();
        }
        return jsonOptions.SerializerOptions;

    }
    /// <inheritdoc/>
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var logger = GetLogger(httpContext.RequestServices);

        httpContext.Response.StatusCode = StatusCode;

        if (!TrySelectPattern(
            httpContext,
            _contentType,
            out var contentType,
            out var options))
            throw new InvalidOperationException();

        httpContext.Response.ContentType = contentType;

        var serializerOptions = GetOptions(httpContext.RequestServices, logger);
#if !NET8_0_OR_GREATER
        serializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
#endif
        var values = ToAsyncEnumerable(httpContext.RequestAborted);

        if (options.IsInvalid)
        {
            var jsonTypeInfo = serializerOptions.GetTypeInfo<IAsyncEnumerable<T>>();

            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                values,
                jsonTypeInfo,
                httpContext.RequestAborted);

            return;
        }

        var elementTypeInfo = serializerOptions.GetTypeInfo<T>();

        await SequenceSerializer.SerializeAsync(
            httpContext.Response.BodyWriter,
            values,
            elementTypeInfo,
            options,
            httpContext.RequestAborted);
    }

}