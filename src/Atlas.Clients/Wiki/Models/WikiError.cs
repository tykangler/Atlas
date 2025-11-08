namespace Atlas.Clients.Wiki.Models;

public record WikiError(
    string Code,
    string Text,
    string Module);