namespace Atlas.Core.Tokenizer.Token;

public record SectionToken(string Value) : WikiToken
{
    public override void Accept(TokenVisitor visitor) => visitor.VisitSection(this);
}
