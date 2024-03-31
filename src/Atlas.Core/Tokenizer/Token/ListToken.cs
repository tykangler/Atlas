using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public record ListToken(IEnumerable<ListItemToken> ListItems, ListType ListType) : WikiToken
{
    public override void Accept(TokenVisitor visitor) => visitor.VisitList(this);
}
