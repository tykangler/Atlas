using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Parse.Token;

public class SectionNode : WikiToken
{
    private const string sectionHeadingClass = "mw-headline";

    public string Value { get; }

    private static bool Validate(IElement elem)
    {
        return elem.ClassList.Contains(sectionHeadingClass);
    }

    public static SectionNode? TryParse(IElement elem)
    {
        if (Validate(elem))
        {
            return new SectionNode(elem.Text().NormalizeWhiteSpace());
        }
        return null;
    }

    public SectionNode(string value) => Value = value;

    public override void Accept(TokenVisitor visitor) => visitor.VisitSection(this);
}
