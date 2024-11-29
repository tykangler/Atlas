using System;

namespace Atlas.Core.Model.Annotations;

public abstract record Annotation(
    int StartIndex,
    int EndIndex,
    string Text,
    AnnotationType Label
);
