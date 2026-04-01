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
using System.Net.Mime;
#endif

namespace Juner.AspNetCore.Sequence.Http.HttpResults;

public sealed class JsonSequenceResult<T> : SequenceResultBase<T>, IEndpointMetadataProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    public JsonSequenceResult(IEnumerable<T> values) : base(values) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    public JsonSequenceResult(IAsyncEnumerable<T> values) : base(values) { }

    public JsonSequenceResult(ChannelReader<T> values) : base(values) { }

    protected override ISequenceSerializerWriteOptions Options => SequenceSerializerOptions.JsonSequence;

    #region StatusCode
    const int STATUS_CODE = StatusCodes.Status200OK;
    /// <summary>
    /// Gets the HTTP status code: <see cref="StatusCodes.Status200OK"/>
    /// </summary>

    public override int StatusCode => STATUS_CODE;
    #endregion

    #region ContentType
    const string CONTENT_TYPE =
#if NET8_0_OR_GREATER
        MediaTypeNames.Application.JsonSequence;
#else
        "application/json-seq";
#endif
    /// <summary>
    /// json-seq content type
    /// </summary>
    public override string ContentType => CONTENT_TYPE;
    #endregion

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
    protected override ILogger GetLogger(IServiceProvider provider) => provider.GetService<ILogger<JsonSequenceResult<T>>>() ?? (ILogger)NullLogger.Instance;
}