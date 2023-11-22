using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Tokenizer.Filters;
using System.Diagnostics.Contracts;

namespace Atlas.Core.Tokenizer;

public static class ElementTokenizer
{
    /// <summary>
    /// Tokenizes an html node into a list of <see cref="WikiToken"/> objects. 
    /// </summary>
    /// <param name="rootElement"></param>
    /// <remarks>Although we are acting on the root, we actually just iterate through the children and tokenize those elements.</remarks>
    public static async Task<List<WikiToken>> Tokenize(INode rootElement)
    {
        var wikiNodes = new List<WikiToken>();
        var filteredNodes = HtmlElementFilter.FilterElements(rootElement.ChildNodes);
        foreach (var childHtmlNode in filteredNodes)
        {
            var token = await TokenFactory.Create(childHtmlNode);
            var toAdd = token == null ? await Tokenize(childHtmlNode) : new List<WikiToken> { token };
            MergeWikiNodes(wikiNodes, toAdd);
        }

        return wikiNodes;
    }

    /// <summary>
    /// Merges wiki nodes. And handles merging text nodes if present. 
    /// </summary>
    /// <returns></returns>
    /// <remarks>Handles merging wiki nodes</remarks>
    private static void MergeWikiNodes(List<WikiToken> initialTokens, List<WikiToken> toAdd)
    {
        if (toAdd.Count > 0 && toAdd[0] is TextNode textNode &&
            initialTokens.Count > 0 && initialTokens[^1] is TextNode lastTextNode)
        {
            initialTokens[^1] = MergeTextNodes(lastTextNode, textNode);
            initialTokens.AddRange(toAdd.Skip(1));
        }
        else
        {
            initialTokens.AddRange(toAdd);
        }
    }

    private static TextNode MergeTextNodes(TextNode node1, TextNode node2) => new($"{node1.Value} {node2.Value}");
}