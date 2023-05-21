using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Models;

namespace Atlas.Core.Tokenizer.Token;

public class ListNode : WikiToken
{
    private const string orderedListTag = "OL";
    private const string unorderedListTag = "UL";
    private const string listTag = "LI";

    public IEnumerable<ListItem> ListItems { get; }

    private static bool Validate(INode node)
    {
        return node is IElement elem &&
            (elem.TagName == orderedListTag || elem.TagName == unorderedListTag);
    }

    public static async Task<ListNode?> TryParse(INode node)
    {
        if (node is IElement element && Validate(node))
        {
            var listItems = await Task.WhenAll(
                element.Children
                    .Where(child => child.TagName == listTag)
                    .Select(TokenFactory.Create));

            if (listItems?.Any() ?? false)
            {
                return new ListNode(listItems.Cast<ListItem>());
            }
        }
        return null;
    }

    public ListNode(IEnumerable<ListItem> listItems) => ListItems = listItems;

    public override void Accept(TokenVisitor visitor) => visitor.VisitList(this);
}
