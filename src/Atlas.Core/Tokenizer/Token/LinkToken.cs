using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public record LinkToken(string Url, string Value, bool IsInterlink) : WikiToken
{
    public override void Accept(TokenVisitor visitor) => visitor.VisitLink(this);
}
