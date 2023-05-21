using Atlas.Core.Tokenizer.Input;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer;

public interface ITokenizer
{
    public Task<IEnumerable<WikiToken>> Tokenize(Document document);
}