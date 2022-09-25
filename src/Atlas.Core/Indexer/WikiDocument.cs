namespace Atlas.Core.Wiki.Extract;

using AST;

/// <summary>
/// Contains tree for wiki document and metadata
/// </summary>
public class WikiDocument
{
    public string Url { get; }
    public WikiNode? Root { get; } = null;
    public IEnumerable<string> Categories { get; }

    public WikiDocument(string url, WikiNode? root, IEnumerable<string> categories)
    {
        Url = url;
        Root = root;
        Categories = categories;
    }
}