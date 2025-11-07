namespace Atlas.Core.Clients.CoreferenceResolver.Models;

/// <summary>
/// Represents a cluster of coreferent mentions that refer to the same entity.
/// </summary>
public record CorefCluster(
    IEnumerable<Mention> Mentions,
    Mention Antecedent
);
