namespace Atlas.Clients.Wiki.Models;

public abstract record BaseWikiResponse(
    IEnumerable<WikiError>? Errors,
    IEnumerable<WikiError>? Warnings
);