namespace Juner.AspNetCore.Sequence.Http;

/// <summary>
/// Marker interface for HTTP results that represent sequence payloads.
/// Implementations typically serialize streaming sequences to the response body.
/// </summary>
public interface ISequenceHttpResult { }