using Atlas.Core.Clients.Narrator;
using Atlas.Core.Clients.Narrator.Models;
using Atlas.Core.Model;
using Atlas.Core.Model.Annotations;

namespace Atlas.Core.Narration;

/// <summary>
/// Makes inferences as to which 
/// </summary>
public class BatchedNarrator : INarrator
{
    private readonly INarratorService narratorService;

    public BatchedNarrator(INarratorService narratorService) => this.narratorService = narratorService;

    public async Task<Document> Narrate(Document document)
    {
        if (string.IsNullOrWhiteSpace(document.Parsed))
        {
            throw new ArgumentException("Document must be parsed (i.e. document.Parsed must be populated.)", nameof(document));
        }
        var relationships = narratorService.GetRelationships(new DocumentRequest(
            Text: document.Parsed,
            TargetPhrases: document.Annotations
                .Where(a => a.Label == AnnotationType.Link)
                .Select(a => new Phrase(Text: a.Text, StartIndex: a.StartIndex, EndIndex: a.EndIndex))
        ));
        await foreach (var relationship in relationships)
        {

        }
    }
}