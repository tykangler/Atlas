using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public class SectionToken : WikiToken
{
    public string Value { get; }

    public SectionToken(string value) => Value = value;

    public override void Accept(TokenVisitor visitor) => visitor.VisitSection(this);
}
