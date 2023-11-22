using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class ListToken : WikiToken
{
    private const string orderedListTag = "OL";
    private const string unorderedListTag = "UL";
    private const string listTag = "LI";

    public IEnumerable<ListToken> ListItems { get; }

    public ListType ListType { get; }

    private static bool Validate(INode node)
    {
        return node is IElement elem &&
            (elem.TagName == orderedListTag || elem.TagName == unorderedListTag);
    }

    public static ListToken? TryParse(INode node)
    {
        if (node is IElement element && Validate(node))
        {
            var listItems = element.Children
                    .Where(child => child.TagName == listTag)
                    .Select(TokenFactory.Create);

            if (listItems?.Any() ?? false)
            {
                return new ListToken(listItems.Cast<ListToken>(), element.TagName == "OL" ? ListType.OrderedList : ListType.UnorderedList);
            }
        }
        return null;
    }

    public ListToken(IEnumerable<ListToken> listItems, ListType listType)
    {
        ListItems = listItems;
        ListType = listType;
    }

    public override void Accept(TokenVisitor visitor) => visitor.VisitList(this);
}
