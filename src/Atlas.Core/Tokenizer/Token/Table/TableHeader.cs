namespace Atlas.Core.Tokenizer.Token.Table;

public record TableHeader(IEnumerable<WikiToken> Tokens, int ColSpan, IEnumerable<TableHeader>? ChildHeaders = null)
{
    public IEnumerable<TableHeader> ChildHeaders { get; init; } = ChildHeaders ?? Enumerable.Empty<TableHeader>();
}
