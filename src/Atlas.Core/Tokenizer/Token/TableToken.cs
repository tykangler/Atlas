using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token.Table;

namespace Atlas.Core.Tokenizer.Token;

public record TableToken(TableHeaderRow Headers, IEnumerable<TableDataRow> Data) : WikiToken
{
    public override void Accept(TokenVisitor visitor) => visitor.VisitTable(this);
}
