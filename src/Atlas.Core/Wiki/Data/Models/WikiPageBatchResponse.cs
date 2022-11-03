using System.Text.Json.Serialization;
using Atlas.Core.Wiki.Data.Converters;

namespace Atlas.Core.Wiki.Data.Models;

[JsonConverter(typeof(WikiPageBatchResponseConverter))]
public record WikiPageBatchResponse(
    IEnumerable<WikiPage> Pages,
    string Continue,
    IEnumerable<WikiError> Errors
) : BaseWikiResponse(Errors);
