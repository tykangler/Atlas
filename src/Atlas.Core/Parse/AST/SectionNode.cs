using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Parse.AST;

public class SectionNode : WikiNode
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

    public override void Accept(ASTVisitor visitor) => visitor.VisitSection(this);
}
