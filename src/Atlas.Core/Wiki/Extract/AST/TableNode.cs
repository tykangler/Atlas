using HtmlAgilityPack;

namespace Atlas.Core.Wiki.Extract.AST;

public class TableNode : WikiNode
{
    internal static bool DoesMatch(HtmlNode node)
    {
        return true;
    }

    internal static TableNode Parse(HtmlNode node)
    {
        return null;
    }

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitTable(this);
    }
}
