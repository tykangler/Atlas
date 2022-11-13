namespace Atlas.Core.Wiki.Parse;

using Atlas.Core.Wiki.Parse.AST;

public interface IWikiParser
{
    public Task<IEnumerable<WikiNode>> Extract(string doc);
}