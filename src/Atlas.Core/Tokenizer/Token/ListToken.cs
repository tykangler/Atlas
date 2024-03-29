using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class ListToken : WikiToken
{
    public IEnumerable<ListItemToken> ListItems { get; }

    public ListType ListType { get; }

    public ListToken(IEnumerable<ListItemToken> listItems, ListType listType)
    {
        ListItems = listItems;
        ListType = listType;
    }

    public override void Accept(TokenVisitor visitor) => visitor.VisitList(this);
}
