using Atlas.Core.Clients.Generated;
using Grpc.Core;
using static Atlas.Core.Clients.Generated.Narrator;

namespace Atlas.Core.Clients.Narrator;

public class NarratorService
{
    private readonly NarratorClient narratorClient;

    public NarratorService(NarratorClient narratorClient)
    {
        this.narratorClient = narratorClient;
    }

    public async IAsyncEnumerable<RelationshipResponse> GetRelationships()
    {
        using var call = narratorClient.GetRelationships(new DocumentRequest
        {
            Text = "abcd",
            TargetPhrases = { }
        });

        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            yield return response;
        }
    }
}