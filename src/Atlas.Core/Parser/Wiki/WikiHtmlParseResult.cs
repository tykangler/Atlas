using Atlas.Core.Model.Annotations;

namespace Atlas.Core.Parser;

public record WikiHtmlParseResult(string Parsed, AnnotationCollection Annotations)
{
    public static WikiHtmlParseResult Empty => new WikiHtmlParseResult(string.Empty, AnnotationCollection.Empty);
}
