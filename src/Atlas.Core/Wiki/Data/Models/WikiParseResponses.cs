namespace Atlas.Core.Wiki.Data.Models;

public record WikiCategory(
    string Category,
    bool Hidden);

public record WikiRedirect(
    string From,
    string To);

public record WikiParseResponse(
    string Title,
    uint PageId,
    IEnumerable<WikiRedirect> Redirects,
    string Text,
    IEnumerable<WikiCategory> Categories);