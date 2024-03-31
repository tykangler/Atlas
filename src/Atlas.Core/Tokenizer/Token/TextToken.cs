using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public record TextToken(string Value) : WikiToken
{
    public override void Accept(TokenVisitor visitor) => visitor.VisitText(this);
}
