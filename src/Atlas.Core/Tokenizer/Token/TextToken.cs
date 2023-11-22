using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public class TextToken : WikiToken
{
    public string Value { get; init; }

    private static bool DoesMatch(INode node) =>
        node.NodeType == NodeType.Text && !string.IsNullOrWhiteSpace(node.Text());

    public static TextToken? TryParse(INode node)
    {
        if (DoesMatch(node))
        {
            string cleanedText = ReplaceNewlineLiterals(node.TextContent).NormalizeWhiteSpace();
            if (!string.IsNullOrWhiteSpace(cleanedText))
            {
                return new TextToken(cleanedText);
            }
        }
        return null;
    }

    private static string ReplaceNewlineLiterals(string s) => s.Replace(@"\n", " ");

    public TextToken(string value) => Value = value;

    public override void Accept(TokenVisitor visitor)
    {
        visitor.VisitText(this);
    }

}
