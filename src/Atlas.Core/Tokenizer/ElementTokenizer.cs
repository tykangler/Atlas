using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Tokenizer.Validators;

namespace Atlas.Core.Tokenizer;

public static class ElementTokenizer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootElement"></param>
    /// <remarks>
    /// What does it even mean to tokenize an html element? Let's say you are trying to tokenize an element with
    /// 4 children. What this really means is you are tokenizing the content of the 4 children, and collapsing it all into
    /// single token. And this token may have 4 children, or it may just have 2 children, or whatever. 
    /// 
    /// I guess the point is, you don't have to do anything with the root. It's all in the children.
    /// </remarks>
    /// <returns></returns>
    public static async Task<List<WikiToken>> TokenizeElement(INode rootElement)
    {
        var wikiNodes = new List<WikiToken>();
        foreach (var childHtmlNode in rootElement.ChildNodes)
        {
            bool elementValid = HtmlElementValidator.IsElementValid(childHtmlNode);
            if (elementValid)
            {
                var token = await TokenFactory.Create(childHtmlNode);
                if (token == null && token is IElement childElement) // can recurse down further
                {
                    var toAdd = await TokenizeElement(childElement);
                    MergeWikiNodes(wikiNodes, toAdd);
                }
                else if (token is TextNode textNode && wikiNodes[^1] is TextNode previousTextNode)
                {
                    wikiNodes[^1] = MergeTextNodes(previousTextNode, textNode);
                }
                else if (token != null) // is valid
                {
                    wikiNodes.Add(token);
                }
            }
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