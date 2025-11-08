using System;

namespace Atlas.Indexer.Models.Annotations;

public record SectionAnnotation(
    int StartIndex,
    int EndIndex,
    string Text,
    int SectionLevel
) : Annotation(StartIndex, EndIndex, Text, AnnotationType.Section);
