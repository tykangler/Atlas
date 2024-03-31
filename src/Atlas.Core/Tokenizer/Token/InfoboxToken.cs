namespace Atlas.Core.Tokenizer.Token;

public record InfoboxToken : WikiToken
{
    public override void Accept(TokenVisitor visitor) => visitor.VisitInfobox(this);
}