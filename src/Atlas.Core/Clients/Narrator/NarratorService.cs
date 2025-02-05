using Atlas.Core.Clients.Narrator.Models;
using Grpc.Core;
using static Atlas.Core.Clients.Generated.Narrator;

namespace Atlas.Core.Clients.Narrator;

/// <summary>
/// Client for the narrator service, which makes inferences as to the actions and relationships that are occuring within a body of text, given
/// a set of target phrases.
/// </summary>
/// <remarks>
/// Note that this is a wrapper class over the generated grpc client for now. But we have the wrapper in case we would like to 
/// </remarks>
public class GrpcNarratorService : INarratorService
{
    private readonly NarratorClient narratorClient;

    public GrpcNarratorService(NarratorClient narratorClient)
    {
        this.narratorClient = narratorClient;
    }

    public async IAsyncEnumerable<RelationshipResponse> GetRelationships(DocumentRequest documentRequest)
    {
        var request = MapDocumentRequest(documentRequest);
        using var call = narratorClient.GetRelationships(request);
        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            yield return MapRelationshipResponse(response);
        }
    }

    private Generated.DocumentRequest MapDocumentRequest(DocumentRequest documentRequest)
    {
        var request = new Generated.DocumentRequest { Text = documentRequest.Text };
        request.TargetPhrases.AddRange(documentRequest.TargetPhrases.Select(phrase => new Generated.Phrase
        {
            Text = phrase.Text,
            StartIndex = phrase.StartIndex,
            EndIndex = phrase.EndIndex,
        }));
        return request;
    }

    private RelationshipResponse MapRelationshipResponse(Generated.RelationshipResponse response)
    => new RelationshipResponse(
        Action: response.Action,
        Prep: response.Prep,
        DetailedAction: response.DetailedAction,
        Target: response.Target
    );
}