using Atlas.Core.Parser.Relationships;
using Atlas.Core.Tokenizer;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Parser;

/// <summary>
/// What should be the goal of the RelationshipParser? What kinds of relationships do we want?
/// I think we would want:
/// 1. Basic Verbs (killed, met, influenced, acted in, directed, produced, etc.)
/// 2. 
/// </summary>
public class RelationshipParser
{
    public IEnumerable<Relationship> ParseRelationships(WikiDocument document)
    {
        var tokens = document.WikiTokens;
        return null;
    }
}
