using AngleSharp.Dom;

namespace Atlas.Core.Wiki.Extract.AST;

public class ListNode : WikiNode
{
    private const string orderedListTag = "OL";
    private const string unorderedListTag = "UL";
    private const string listTag = "LI";

    public IEnumerable<WikiNode> ListItems { get; }
    private static bool Validate(IElement elem)
    {
        return elem.TagName == orderedListTag || elem.TagName == unorderedListTag;
    }

    public static ListNode? TryParse(IElement elem, Func<IElement, List<WikiNode>> extractFunc)
    {
        if (Validate(elem))
        {
            var listItems = elem.Children
                .Where(child => child.TagName == listTag)
                .SelectMany(extractFunc);

            if (listItems.Any())
            {
                return new ListNode(listItems);
            }
        }
        return null;
    }

    public ListNode(IEnumerable<WikiNode> listItems) => ListItems = listItems;

    public override void Accept(ASTVisitor visitor) => visitor.VisitList(this);
}
