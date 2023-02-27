using AngleSharp.Dom;

namespace Atlas.Core.Wiki.Annotator.Token;

public class ListNode : WikiToken
{
    private const string orderedListTag = "OL";
    private const string unorderedListTag = "UL";
    private const string listTag = "LI";

    public IEnumerable<WikiToken> ListItems { get; }
    private static bool Validate(IElement elem)
    {
        return elem.TagName == orderedListTag || elem.TagName == unorderedListTag;
    }

    public static ListNode? TryParse(IElement elem, Func<IElement, List<WikiToken>> extractFunc)
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

    public ListNode(IEnumerable<WikiToken> listItems) => ListItems = listItems;

    public override void Accept(TokenVisitor visitor) => visitor.VisitList(this);
}
