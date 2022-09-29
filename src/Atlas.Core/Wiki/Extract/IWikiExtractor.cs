namespace Atlas.Core.Wiki.Extract.AST;

public interface IWikiExtractor
{
    public Task<IEnumerable<WikiNode>> Extract(string doc);
}