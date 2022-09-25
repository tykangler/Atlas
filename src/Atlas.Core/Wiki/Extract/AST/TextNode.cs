using HtmlAgilityPack;

namespace Atlas.Core.Wiki.Extract.AST;

public class TextNode : WikiNode
{
    public string Value { get; }

    internal static bool DoesMatch(HtmlNode node)
    {
        string[] disallowTags = {
            "a", "table", "style", "sup"
        };

        return node.NodeType == HtmlNodeType.Text;
    }

    internal static TextNode? Parse(HtmlNode node)
    {
        string cleanedText = CleanText(node.InnerText);
        if (!string.IsNullOrWhiteSpace(cleanedText))
        {
            return new TextNode(cleanedText);
        }
        return null;
    }

    internal static string CleanText(string text) =>
        text.Replace(@"\n", "")
            .Trim();

    public TextNode(string value) => this.Value = value;

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitText(this);
    }
}
