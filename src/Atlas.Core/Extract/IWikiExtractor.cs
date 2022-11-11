using Atlas.Core.Wiki.Extract.AST;

namespace Atlas.Core.Wiki.Extract;

public interface IWikiExtractor
{
    public Task<IEnumerable<WikiNode>> Extract(string doc);
}