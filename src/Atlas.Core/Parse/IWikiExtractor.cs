namespace Atlas.Core.Wiki.Parse;

using Atlas.Core.Wiki.Parse.Token;

public interface IWikiParser
{
    public Task<IEnumerable<WikiToken>> Extract(string doc);
}