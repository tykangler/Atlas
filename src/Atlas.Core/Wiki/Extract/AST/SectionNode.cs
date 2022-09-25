using HtmlAgilityPack;

namespace Atlas.Core.Wiki.Extract.AST;

public class SectionNode : WikiNode
{
    internal static bool DoesMatch(HtmlNode node)
    {
        return true;
    }

    internal static SectionNode Parse(HtmlNode node)
    {
        return null;
    }

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitSection(this);
    }
}
