namespace Atlas.Clients.Wiki.Models;

public record WikiParse(
    string Title,
    int PageId,
    IEnumerable<WikiRedirect> Redirects,
    string Text,
    IEnumerable<WikiCategory> Categories
);