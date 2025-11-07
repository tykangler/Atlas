using Atlas.Core.Clients.CoreferenceResolver.Models;
using static Atlas.Core.Clients.Generated.CoreferenceResolver;

namespace Atlas.Core.Clients.CoreferenceResolver;

/// <summary>
/// gRPC client for the coreference resolution service.
/// </summary>
/// <remarks>
/// This is a wrapper class over the generated gRPC client to provide a clean interface
/// and handle mapping between domain models and protobuf messages.
/// </remarks>
public class GrpcCoreferencService : ICoreferencService
{
    private readonly CoreferenceResolverClient coreferenceClient;

    public GrpcCoreferencService(CoreferenceResolverClient coreferenceClient)
    {
        this.coreferenceClient = coreferenceClient;
    }

    public async Task<CoreferenceResponse> ResolveCoreferences(CoreferenceRequest coreferenceRequest)
    {
        var request = MapCoreferencRequest(coreferenceRequest);
        var response = await coreferenceClient.ResolveCoreferencesAsync(request);
        return MapCoreferencResponse(response);
    }

    private Generated.CoreferenceRequest MapCoreferencRequest(CoreferenceRequest request)
        => new Generated.CoreferenceRequest { Text = request.Text };

    private CoreferenceResponse MapCoreferencResponse(Generated.CoreferenceResponse response)
        => new CoreferenceResponse(
            Clusters: response.Clusters.Select(MapCorefCluster)
        );

    private CorefCluster MapCorefCluster(Generated.CorefCluster cluster)
        => new CorefCluster(
            Mentions: cluster.Mentions.Select(MapMention),
            Antecedent: MapMention(cluster.Antecedent)
        );

    private Mention MapMention(Generated.Mention mention)
        => new Mention(
            StartChar: mention.StartChar,
            EndChar: mention.EndChar,
            Text: mention.Text
        );
}
