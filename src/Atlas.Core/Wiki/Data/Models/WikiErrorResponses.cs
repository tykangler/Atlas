namespace Atlas.Core.Wiki.Data.Models;

public record WikiErrorResponse(
    string Code,
    string Text,
    string Module);

