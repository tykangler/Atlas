using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public class SectionToken : WikiToken
{
    private const string sectionHeadingClass = "mw-headline";

    public string Value { get; }

    private static bool Validate(INode node)
    {
        return node is IElement elem && elem.ClassList.Contains(sectionHeadingClass);
    }

    public static SectionToken? TryParse(INode node)
    {
        if (Validate(node))
        {
            return new SectionToken(node.Text().NormalizeWhiteSpace());
        }
        return null;
    }

    public SectionToken(string value) => Value = value;

    public override void Accept(TokenVisitor visitor) => visitor.VisitSection(this);
}
