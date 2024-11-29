using System;
using Atlas.Core.Model.Annotations;

namespace Atlas.Core.Model;

public record Document(
    string Raw,
    string Parsed,
    AnnotationCollection Annotations);
