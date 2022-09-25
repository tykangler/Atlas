using HtmlAgilityPack;

namespace Atlas.Core.Wiki.Extract.AST;

public class ListNode : WikiNode
{
    internal static bool DoesMatch(HtmlNode node)
    {
        return true;
    }

    internal static ListNode Parse(HtmlNode node)
    {
        return null;
    }

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitList(this);
    }
}
