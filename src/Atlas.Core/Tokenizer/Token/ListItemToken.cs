using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class ListItemToken : WikiToken
{
    public IEnumerable<WikiToken> Tokens { get; }

    public ListItemToken(IEnumerable<WikiToken> tokens) => Tokens = tokens;

    public override void Accept(TokenVisitor visitor) => visitor.VisitListItem(this);
}
