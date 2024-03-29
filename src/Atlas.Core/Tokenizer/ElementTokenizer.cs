using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Tokenizer.Filters;
using System.Collections.Immutable;

namespace Atlas.Core.Tokenizer;

public static class ElementTokenizer
{
    /// <summary>
    /// Tokenizes children of the root node into a list of <see cref="WikiToken"/> objects. Elements are filtered based on <see cref="HtmlElementFilter"/>. 
    /// The passed <see cref="rootNode"/> is excluded, so that the final result is a list of tokens corresponding to the children of the root node.
    /// </summary>
    /// <param name="rootElement"></param>
    public static IEnumerable<WikiToken> TokenizeChildren(INode rootNode) => TokenizeNodes(rootNode.ChildNodes);

    /// <summary>
    /// Tokenizes a list of nodes into wiki tokens. Note that the final result may not be a 1:1 mapping because <see cref="HtmlElementFilter"/> will filter
    /// out noisy nodes.
    /// </summary>
    /// <param name="nodes">Nodes to tokenize</param>
    /// <returns></returns>
    public static IEnumerable<WikiToken> TokenizeNodes(IEnumerable<INode> nodes)
        => HtmlElementFilter.Filter(nodes)
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
        return token == null // if token is container, it can't be invalid since we already filtered elements
            ? TokenizeChildren(node)
            : ImmutableList.Create(token);
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