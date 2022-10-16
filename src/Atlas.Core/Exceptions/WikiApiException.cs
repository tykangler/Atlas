using Atlas.Core.Wiki.Data.Models;

namespace Atlas.Core.Exceptions;

[Serializable]
public class WikiApiException : Exception
{
    public IEnumerable<WikiErrorResponse>? Errors { get; }

    public WikiApiException() { }

    public WikiApiException(string message) : base(message) { }

    public WikiApiException(string message, Exception inner) : base(message, inner) { }

    public WikiApiException(string message, IEnumerable<WikiErrorResponse> errors) : this(message) =>
        this.Errors = errors;
}