namespace Atlas.Core.Clients.CoreferenceResolver.Models;

/// <summary>
/// Response containing coreference clusters.
/// </summary>
public record CoreferenceResponse(
    IEnumerable<CorefCluster> Clusters
);
