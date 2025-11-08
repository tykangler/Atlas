using Atlas.Clients.Narrator.Models;

namespace Atlas.Clients.Narrator;

public interface INarratorService
{
    public IAsyncEnumerable<RelationshipResponse> GetRelationships(DocumentRequest documentRequest);
}