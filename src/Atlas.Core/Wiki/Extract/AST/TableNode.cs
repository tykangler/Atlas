using AngleSharp.Dom;

namespace Atlas.Core.Wiki.Extract.AST;

public class TableNode : WikiNode
{
    public IEnumerable<string> TableHeaders { get; }
    public IEnumerable<string> TableData { get; }

    private static bool Validate(IElement elem)
    {
        return elem.TagName == "table";
    }

    internal static bool TryParse(IElement elem, out TableNode? wikiNode)
    {
        if (Validate(elem))
        {
            wikiNode = null;
            return true;
        }
        wikiNode = null;
        return false;
    }

    public TableNode(IEnumerable<string> headers, IEnumerable<string> data)
    {
        TableHeaders = headers;
        TableData = data;
    }

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitTable(this);
    }
}
