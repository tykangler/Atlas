using Atlas.Core.Wiki.Data;

namespace Atlas.Core.Exceptions;

[Serializable]
public class WikiApiException : Exception
{
    WikiErrorResponse? Error { get; }

    public WikiApiException() { }

    public WikiApiException(string message) : base(message) { }

    public WikiApiException(string message, Exception inner) : base(message, inner) { }

    public WikiApiException(string message, WikiErrorResponse error) : this(message) =>
        this.Error = error;
}