namespace Atlas.Core.Clients.CoreferenceResolver.Models;

/// <summary>
/// Represents a mention in text that refers to an entity.
/// </summary>
public record Mention(
    int StartChar,
    int EndChar,
    string Text
);
