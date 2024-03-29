using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public interface IHandler
{
    public bool CanHandle(INode node);

    public WikiToken? Handle(INode node);
}
