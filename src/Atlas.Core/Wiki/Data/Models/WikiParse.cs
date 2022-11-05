namespace Atlas.Core.Wiki.Data.Models;

public record WikiParse(
    string Title,
    int PageId,
    IEnumerable<WikiRedirect> Redirects,
    string Text,
    IEnumerable<WikiCategory> Categories
);