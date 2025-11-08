using Atlas.Clients.Wiki.Models;

namespace Atlas.Clients.Wiki.Exceptions;

[Serializable]
public class WikiApiException : Exception
{
    public IEnumerable<WikiError>? Errors { get; }

    public WikiApiException() { }

    public WikiApiException(string message) : base(message) { }

    public WikiApiException(string message, Exception inner) : base(message, inner) { }

    public WikiApiException(string message, IEnumerable<WikiError>? errors) : this(message) =>
        this.Errors = errors;
}