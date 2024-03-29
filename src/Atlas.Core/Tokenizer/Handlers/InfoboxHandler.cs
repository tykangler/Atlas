using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class InfoboxHandler : IHandler
{
    public bool CanHandle(INode node)
    {
        return false;
    }

    public WikiToken Handle(INode node)
    {
        throw new NotImplementedException();
    }

}
