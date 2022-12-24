using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Parse.Token;

public class TextNode : WikiToken
{
    public string Value { get; }

    private static bool DoesMatch(INode node) =>
        node.NodeType == NodeType.Text && !string.IsNullOrWhiteSpace(node.Text());

    public static TextNode? TryParse(INode node)
    {
        if (DoesMatch(node))
        {
            string cleanedText = ReplaceNewlineLiterals(node.TextContent).NormalizeWhiteSpace();
            if (!string.IsNullOrWhiteSpace(cleanedText))
            {
                return new TextNode(cleanedText);
            }
        }
        return null;
    }

    private static string ReplaceNewlineLiterals(string s) => s.Replace(@"\n", " ");

    public TextNode(string value) => this.Value = value;

    public override void Accept(TokenVisitor visitor)
    {
        visitor.VisitText(this);
    }

}
