namespace Atlas.Core.Tokenizer.Token.Table;

public record TableData(IEnumerable<WikiToken> CellData, int RowSpan, int ColSpan)
{
}
