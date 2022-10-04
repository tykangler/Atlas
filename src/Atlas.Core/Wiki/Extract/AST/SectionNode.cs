using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Extract.AST;

public class SectionNode : WikiNode
{
    private const string sectionHeadingClass = "mw-headline";

    public string Value { get; }

    private static bool Validate(IElement elem)
    {
        return elem.ClassList.Contains(sectionHeadingClass);
    }

    public static bool TryParse(IElement elem, out SectionNode? wikiNode)
    {
        if (Validate(elem))
        {
            wikiNode = new SectionNode(elem.Text().NormalizeWhiteSpace());
            return true;
        }
        wikiNode = null;
        return false;
    }

    public SectionNode(string value) => Value = value;

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitSection(this);
    }
}
