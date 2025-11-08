using Atlas.Indexer.Models.Annotations;

namespace Atlas.Indexer.Parsing;

public record WikiHtmlParseResult(string Parsed, AnnotationCollection Annotations)
{
    public static WikiHtmlParseResult Empty => new WikiHtmlParseResult(string.Empty, AnnotationCollection.Empty);
}
