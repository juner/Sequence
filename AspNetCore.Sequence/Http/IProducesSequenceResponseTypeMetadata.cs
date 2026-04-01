using Microsoft.AspNetCore.Http.Metadata;

namespace Juner.AspNetCore.Sequence.Http;

public interface IProducesSequenceResponseTypeMetadata : IProducesResponseTypeMetadata
{
    /// <summary>
    /// Gets the optimistic sequence return type of the action.
    /// </summary>
    Type? ItemType { get; }

    /// <summary>
    /// Gets the list of supported content types and associated metadata for the response.
    new IReadOnlyList<IContent> ContentTypes { get; }

}