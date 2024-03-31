using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public record ListItemToken(IEnumerable<WikiToken> Tokens) : WikiToken
{
    public override void Accept(TokenVisitor visitor) => visitor.VisitListItem(this);
}
