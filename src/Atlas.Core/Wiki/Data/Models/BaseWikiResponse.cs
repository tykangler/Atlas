namespace Atlas.Core.Wiki.Data.Models;

public abstract record BaseWikiResponse(
    IEnumerable<WikiError>? Errors,
    IEnumerable<WikiError>? Warnings
);