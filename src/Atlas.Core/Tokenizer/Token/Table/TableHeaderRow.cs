using System.Globalization;

namespace Atlas.Core.Tokenizer.Token.Table;

public record TableHeaderRow(IEnumerable<TableHeader> TableHeaders)
{
    private IEnumerable<TableHeader> TableHeaders { get; init; } = TableHeaders;

    /// <summary>
    /// Returns all headers in a pre-order traversal. For a hierarchal structure, with h1, h2, h3. And children
    /// h1-h1*, h1-h2*, h2-h1*, h2-h2*, it will return h1, h1-h1*, h1-h2*, h2, h2-h1*, h2-h2*, h3
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TableHeader> GetHeaders() => WalkHeaders(TableHeaders);

    private IEnumerable<TableHeader> WalkHeaders(IEnumerable<TableHeader> headers)
    {
        foreach (var header in headers)
        {
            yield return header;
            if (header.ChildHeaders.Any())
            {
                foreach (var child in WalkHeaders(header.ChildHeaders))
                {
                    yield return child;
                }
            }
        }
    }
}
