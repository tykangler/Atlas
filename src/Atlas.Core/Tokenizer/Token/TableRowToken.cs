using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class TableRowToken : WikiToken
{
    public IDictionary<string, string> RowData { get; }

    public TableRowToken(IDictionary<string, string> rowData)
    {
        RowData = rowData;
    }

    public override void Accept(TokenVisitor visitor)
    {
        throw new NotImplementedException();
    }
}
