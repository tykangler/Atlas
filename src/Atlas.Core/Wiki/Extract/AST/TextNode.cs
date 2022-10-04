using AngleSharp.Dom;

namespace Atlas.Core.Wiki.Extract.AST;

public class TextNode : WikiNode
{
    public string Value { get; }

    private static bool DoesMatch(INode node) =>
        node.NodeType == NodeType.Text && !string.IsNullOrWhiteSpace(node.Text());

    internal static bool TryParse(INode node, out TextNode? wikiNode)
    {
        if (DoesMatch(node))
        {
            string cleanedText = CleanText(node.TextContent);
            if (!string.IsNullOrWhiteSpace(cleanedText))
            {
                wikiNode = new TextNode(cleanedText);
                return true;
            }
        }
        wikiNode = null;
        return false;
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
