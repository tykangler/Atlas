namespace Atlas.Clients.Narrator.Models;

public record DocumentRequest(
    string Text,
    IEnumerable<Phrase> TargetPhrases
);