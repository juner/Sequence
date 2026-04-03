using System.Diagnostics;
using System.Text.Json;

using Juner.AspNetCore.Sequence.Internals;
using Juner.Sequence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


#if !NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif                        

namespace Juner.AspNetCore.Sequence.Http.HttpResults;

[DebuggerDisplay("{Values,nq}")]
public abstract partial class SequenceResultBase<T> : IResult
{
    protected abstract ILogger GetLogger(IServiceProvider provider);
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
        ArgumentNullException.ThrowIfNull(httpContext);

        // Creating the logger with a string to preserve the category after the refactoring.
        var logger = GetLogger(httpContext.RequestServices);
        httpContext.Response.StatusCode = StatusCode;
        if (string.IsNullOrEmpty(httpContext.Response.ContentType))
            httpContext.Response.ContentType = ContentType;

        var serializerOptions = SequenceResultBase<T>.GetOptions(httpContext.RequestServices, logger);
#if !NET8_0_OR_GREATER
        serializerOptions.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
#endif
        var jsonTypeInfo = serializerOptions.GetTypeInfo<T>();
        var cancellationToken = httpContext.RequestAborted;
        var values = ToAsyncEnumerable(cancellationToken);
        await SequenceSerializer.SerializeAsync(
            httpContext.Response.BodyWriter,
            values,
            jsonTypeInfo,
            Options,
            cancellationToken);
    }
}