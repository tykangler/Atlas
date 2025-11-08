namespace Atlas.Clients.Wiki.Models;

public record WikiParseResponse(
    IEnumerable<WikiError> Errors,
    IEnumerable<WikiError> Warnings,
    WikiParse Parse) : BaseWikiResponse(Errors, Warnings);
