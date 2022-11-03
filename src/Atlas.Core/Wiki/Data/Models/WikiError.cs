namespace Atlas.Core.Wiki.Data.Models;

public record WikiError(
    string Code,
    string Text,
    string Module,
    bool IsWarning);