using Atlas.Console.Exceptions;
using Atlas.Core.Clients.Wiki.Models;
using Atlas.Core.Services;

namespace Atlas.Console.Services;

public class PageContentService(WikiApiService apiService)
{
    public async Task<WikiParseResponse?> GetPageContent(string? pageTitle, string? pageId)
    {
        if (!IsOptionsSetValid(pageTitle, pageId))
        {
            throw new InvalidOptionsException("Either page title or page id must be specified but not both");
        }
        if (!string.IsNullOrWhiteSpace(pageTitle))
        {
            return await apiService.GetPageContentFromTitleAsync(pageTitle);
        }
        else if (!string.IsNullOrWhiteSpace(pageId))
        {
            return await apiService.GetPageContentFromIdAsync(pageId);
        }
        return null;
    }

    /// <summary>
    /// Validates that a given list of option values have one and only one value specified. If value is not specified, the value should be null or empty in the options set.
    /// </summary>
    /// <param name="options">options values</param>
    /// <returns>true if the set of options is valid (only one value is specified). false otherwise</returns>
    private static bool IsOptionsSetValid(params string?[] options)
        => options.Count(opt => !string.IsNullOrEmpty(opt)) == 1;
}