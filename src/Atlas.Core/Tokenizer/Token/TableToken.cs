using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class TableToken : WikiToken
{
    public IEnumerable<WikiToken> TableHeaders { get; private set; }

    public IEnumerable<TableRowToken> TableRows { get; private set; }

    public TableToken(IEnumerable<WikiToken> headers, IEnumerable<TableRowToken> data)
    {
        TableHeaders = headers;
        TableRows = data;
    }

    public override void Accept(TokenVisitor visitor) => throw new NotImplementedException();
}
