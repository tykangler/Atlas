using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Tokenizer.Filters;
using System.Collections.Immutable;

namespace Atlas.Core.Tokenizer;

public static class ElementTokenizer
{
    /// <summary>
    /// Tokenizes an html node into a list of <see cref="WikiToken"/> objects. 
    /// </summary>
    /// <param name="rootElement"></param>
    /// <remarks>Although we are acting on the root, we actually just iterate through the children and tokenize those elements.</remarks>
    public static IEnumerable<WikiToken> Tokenize(INode rootElement)
        => HtmlElementFilter
            .Filter(rootElement.ChildNodes)
            .Select(CreateWikiTokens)
            .Aggregate(Enumerable.Empty<WikiToken>(), MergeWikiTokens);

    /// <summary>
    /// Given an html node, creates a list of wiki tokens. The list will have more than one element if the node is a container node,
    /// and will have only one element if it can be mapped directly to a wiki token.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static IEnumerable<WikiToken> CreateWikiTokens(INode node)
    {
        var token = TokenFactory.Create(node);
        return token == null ? Tokenize(node) : ImmutableList.Create(token);
    }

    /// <summary>
    /// Merges wiki nodes into a single node list.
    /// </summary>
    /// <returns></returns>
    /// <remarks> 
    ///     If the last token in <see cref="tokens"/> and the first token in <see cref="toAdd"/> are text nodes, the two nodes will be merged.
    ///     For example, if tokens[^1] is TextNode and toAdd[0] is TextNode, then the end result's last node will be a merged TextNode
    ///     consisting of tokens[^1] + toAdd[0], and toAdd[0] will be ignored.
    /// </remarks>
    private static IEnumerable<WikiToken> MergeWikiTokens(IEnumerable<WikiToken> tokens, IEnumerable<WikiToken> toAdd)
    {
        if (toAdd.Any() && toAdd.First() is TextToken textNode &&
            tokens.Any() && tokens.Last() is TextToken lastTextNode)
        {
            return tokens
                .SkipLast(1)
                .Append(MergeTextNodes(lastTextNode, textNode))
                .Concat(toAdd.Skip(1));
        }
        else
        {
            return tokens.Concat(toAdd);
        }
    }

    private static WikiToken MergeTextNodes(TextToken node1, TextToken node2) => new TextToken($"{node1.Value} {node2.Value}");
}