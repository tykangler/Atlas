using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Extract.AST;

public class ListNode : WikiNode
{
    private const string orderedList = "OL";
    private const string unorderedList = "UL";
    private const string listItem = "LI";

    public IEnumerable<string> ListItems { get; }
    private static bool Validate(IElement elem)
    {
        return elem.TagName == orderedList || elem.TagName == unorderedList;
    }

    public static bool TryParse(IElement elem, out ListNode? wikiNode)
    {
        if (Validate(elem))
        {
            var listItems =
                from child in elem.Children
                where child.TagName == listItem
                select child.Text().NormalizeWhiteSpace();
            wikiNode = new ListNode(listItems);
            return true;
        }
        wikiNode = null;
        return false;
    }

    public ListNode(IEnumerable<string> listItems) => ListItems = listItems;

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitList(this);
    }
}
