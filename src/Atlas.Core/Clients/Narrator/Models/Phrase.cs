namespace Atlas.Core.Clients.Narrator.Models;

public record Phrase(
    string Text,
    int StartIndex,
    int EndIndex
);