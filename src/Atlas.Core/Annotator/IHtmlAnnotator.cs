namespace Atlas.Core.Wiki.Annotator;

using Atlas.Core.Wiki.Annotator.Token;

public interface IHtmlAnnotator
{
    public Task<IEnumerable<WikiToken>> Extract(string doc);
}