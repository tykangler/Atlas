namespace Atlas.Clients.Wiki.Models;

public record WikiGetPageResponse(
    IEnumerable<WikiError> Errors,
    IEnumerable<WikiError> Warnings,
    WikiContinue Continue,
    WikiQuery Query
) : BaseWikiResponse(Errors, Warnings);
