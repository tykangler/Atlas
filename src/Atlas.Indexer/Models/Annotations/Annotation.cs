using System;
using System.Text.Json.Serialization;

namespace Atlas.Indexer.Models.Annotations;

[JsonDerivedType(typeof(LinkAnnotation))]
[JsonDerivedType(typeof(SectionAnnotation))]
public abstract record Annotation(
    int StartIndex,
    int EndIndex,
    string Text,
    AnnotationType Label
);
