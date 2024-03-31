using AngleSharp.Dom;
using Atlas.Core.Extensions;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class ListHandler : IHandler
{
    private const string OrderedListTag = "OL";
    private const string UnorderedListTag = "UL";
    private const string ListItemTag = "LI";

    public bool CanHandle(INode node)
        => node is IElement element && (element.IsTag(OrderedListTag) || element.IsTag(UnorderedListTag));

    public WikiToken? Handle(INode node)
    {
        if (!(node is IElement element && CanHandle(node)))
        {
            return null;
        }
        var listType = element.IsTag(OrderedListTag) ? ListType.OrderedList : ListType.UnorderedList;
        var listItemElements = element.Children
            .Where(child => child.IsTag(ListItemTag));
        var listItems = ElementTokenizer.TokenizeNodes(listItemElements);
        return new ListToken(
            ListItems: listItems.Cast<ListItemToken>(),
            ListType: listType);
    }

}
