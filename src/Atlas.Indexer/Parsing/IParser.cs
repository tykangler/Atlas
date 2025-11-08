using Atlas.Indexer.Models;

namespace Atlas.Indexer.Parsing;

/// <summary>
/// Represents a parser that can convert an input to a <see cref="Document"/> that can be used for internal processing.
/// </summary>
public interface IParser
{
    /// <summary>
    /// Parses <see cref="inputDoc"/> to a <see cref="Document?"/>.
    /// </summary>
    /// <param name="inputDoc"></param>
    /// <exception cref="InvalidOperationException">
    /// <returns></returns>
    public Task<Document> Parse(string inputDoc);
}
