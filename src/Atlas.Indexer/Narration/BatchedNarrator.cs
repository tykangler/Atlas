using Atlas.Clients.Narrator;
using Atlas.Clients.Narrator.Models;
using Atlas.Indexer.Models;
using Atlas.Indexer.Models.Annotations;

namespace Atlas.Indexer.Narration;

/// <summary>
/// Makes inferences as to which 
/// </summary>
public class BatchedNarrator
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
        return null;
    }
}
