using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class TableToken : WikiToken
{
    public IEnumerable<string> TableHeaders { get; }
    public IEnumerable<string> TableData { get; }

    // private static bool Validate(IElement elem)
    // {
    //     return elem.TagName == "table";
    // }

    public static TableToken? TryParse(INode node)
    {
        // default implementation 
        return null;
    }

    public TableToken(IEnumerable<string> headers, IEnumerable<string> data)
    {
        TableHeaders = headers;
        TableData = data;
    }

    public override void Accept(TokenVisitor visitor) => throw new NotImplementedException();
}
