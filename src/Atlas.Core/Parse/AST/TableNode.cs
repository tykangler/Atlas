using AngleSharp.Dom;

namespace Atlas.Core.Wiki.Parse.AST;

public class TableNode : WikiNode
{
    public IEnumerable<string> TableHeaders { get; }
    public IEnumerable<string> TableData { get; }

    private static bool Validate(IElement elem)
    {
        return elem.TagName == "table";
    }

    public static TableNode? TryParse(IElement elem)
    {
        // default implementation 
        return null;
    }

    public TableNode(IEnumerable<string> headers, IEnumerable<string> data)
    {
        TableHeaders = headers;
        TableData = data;
    }

    public override void Accept(ASTVisitor visitor) => throw new NotImplementedException();
}
