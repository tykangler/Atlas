using Atlas.Core.Model;
using Atlas.Core.Model.Annotations;
using Atlas.Core.Model.Graph;

namespace Atlas.Core.Narration;

public record NarratedDocument(
    string Raw,
    string Parsed,
    AnnotationCollection Annotations,
    IEnumerable<Relationship> Relationships)
: Document(Raw, Parsed, Annotations);
