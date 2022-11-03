namespace Atlas.Core.Wiki.Data.Models;

public record WikiParseResponse(
    string Title,
    uint PageId,
    IEnumerable<WikiRedirect> Redirects,
    string Text,
    IEnumerable<WikiCategory> Categories,
    IEnumerable<WikiError> Errors)
: BaseWikiResponse(Errors);