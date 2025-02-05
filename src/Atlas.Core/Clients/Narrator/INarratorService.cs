using Atlas.Core.Clients.Narrator.Models;

namespace Atlas.Core.Clients.Narrator;

public interface INarratorService
{
    public IAsyncEnumerable<RelationshipResponse> GetRelationships(DocumentRequest documentRequest);
}