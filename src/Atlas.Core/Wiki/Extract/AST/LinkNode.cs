using HtmlAgilityPack;

namespace Atlas.Core.Wiki.Extract.AST;

public class LinkNode : WikiNode
{
    internal static bool DoesMatch(HtmlNode node)
    {
        return true;
    }

    internal static LinkNode Parse(HtmlNode node)
    {
        return null;
    }

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitLink(this);
    }
}
