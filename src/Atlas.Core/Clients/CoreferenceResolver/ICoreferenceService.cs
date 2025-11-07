using Atlas.Core.Clients.CoreferenceResolver.Models;

namespace Atlas.Core.Clients.CoreferenceResolver;

/// <summary>
/// Service for resolving coreferences in text.
/// </summary>
public interface ICoreferencService
{
    /// <summary>
    /// Resolves coreferences in the given text, identifying mentions that refer to the same entity.
    /// </summary>
    /// <param name="coreferenceRequest">The request containing the text to analyze.</param>
    /// <returns>A response containing coreference clusters.</returns>
    Task<CoreferenceResponse> ResolveCoreferences(CoreferenceRequest coreferenceRequest);
}
