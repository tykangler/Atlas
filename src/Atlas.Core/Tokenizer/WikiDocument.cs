using Atlas.Core.Tokenizer.Input;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer;

public record WikiDocument(
    Document RawInput,
    IEnumerable<WikiToken> WikiTokens
);
