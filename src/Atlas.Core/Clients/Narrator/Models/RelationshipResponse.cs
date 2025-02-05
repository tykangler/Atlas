namespace Atlas.Core.Clients.Narrator.Models;

public record RelationshipResponse(
    string Action,
    string Prep,
    string DetailedAction,
    string Target
);