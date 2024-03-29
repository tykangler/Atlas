using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public class TextToken : WikiToken
{
    public string Value { get; init; }

    public TextToken(string value) => Value = value;

    public override void Accept(TokenVisitor visitor)
    {
        visitor.VisitText(this);
    }

}
