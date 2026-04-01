using System.Reflection;
using System.Threading.Channels;

using Juner.Sequence;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#if NET8_0_OR_GREATER
#endif

namespace Juner.AspNetCore.Sequence.Http.HttpResults;

public sealed class JsonLineResult<T> : SequenceResultBase<T>, IEndpointMetadataProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    public JsonLineResult(IEnumerable<T> values) : base(values) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    public JsonLineResult(IAsyncEnumerable<T> values) : base(values) { }

    public JsonLineResult(ChannelReader<T> values) : base(values) { }


    #region StatusCode
    const int STATUS_CODE = StatusCodes.Status200OK;
    /// <summary>
    /// Gets the HTTP status code: <see cref="StatusCodes.Status200OK"/>
    /// </summary>

    public override int StatusCode => STATUS_CODE;
    #endregion

    #region ContentType
    const string CONTENT_TYPE =
        "application/jsonl";
    /// <summary>
    /// json-seq content type
    /// </summary>
    public override string ContentType => CONTENT_TYPE;

    #endregion  
    protected override ISequenceSerializerWriteOptions Options => SequenceSerializerOptions.JsonLines;

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesSequenceResponseTypeMetadata(
            STATUS_CODE,
            typeof(T),
            [new Content(CONTENT_TYPE, true)]));
    }

    protected override ILogger GetLogger(IServiceProvider provider) => provider.GetService<ILogger<JsonLineResult<T>>>() ?? (ILogger)NullLogger.Instance;
}