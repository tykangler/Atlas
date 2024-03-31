using AngleSharp.Dom;
using Atlas.Core.Extensions;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class TextHandler : IHandler
{
    public bool CanHandle(INode node)
    {
        return node.NodeType == NodeType.Text && !string.IsNullOrWhiteSpace(node.Text());
    }

    public WikiToken? Handle(INode node)
    {
        if (!CanHandle(node))
        {
            return null;
        }
        string cleanedTextToken = ReplaceNewlineLiterals(node.TextContent)
            .NormalizeWhiteSpace();
        if (string.IsNullOrWhiteSpace(cleanedTextToken))
        {
            return null;
        }
        return new TextToken(cleanedTextToken);
    }

    private static string ReplaceNewlineLiterals(string s) => s.Replace(@"\n", " ");


}
