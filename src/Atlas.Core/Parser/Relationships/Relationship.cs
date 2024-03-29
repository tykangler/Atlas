using Atlas.Core.Parser.Common;

namespace Atlas.Core.Parser.Relationships;

public record Relationship(string Section, string Text, IEnumerable<InterlinkReference> References);