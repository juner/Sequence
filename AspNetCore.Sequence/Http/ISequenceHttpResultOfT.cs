namespace Juner.AspNetCore.Sequence.Http;

/// <summary>
/// Generic marker interface for HTTP results that produce a sequence of <typeparamref name="T"/>.
/// </summary>
public interface ISequenceHttpResult<T> : ISequenceHttpResult
{

}