namespace Atlas.Indexer.Models.Annotations;

public record LinkAnnotation(
    int StartIndex,
    int EndIndex,
    string Text,
    string Link) : Annotation(StartIndex, EndIndex, Text, AnnotationType.Link);
