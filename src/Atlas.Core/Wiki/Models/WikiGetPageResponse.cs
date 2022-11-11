namespace Atlas.Core.Wiki.Models;

public record WikiQuery(IEnumerable<WikiPage> Pages);

public record WikiGetPageResponse(
    IEnumerable<WikiError> Errors,
    IEnumerable<WikiError> Warnings,
    WikiContinue Continue,
    WikiQuery Query
) : BaseWikiResponse(Errors, Warnings);
