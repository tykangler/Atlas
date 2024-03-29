using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class TableHeaderHandler : IHandler
{
    public bool CanHandle(INode node)
    {
        return false;
        // throw new NotImplementedException();
    }

    public WikiToken Handle(INode node)
    {
        throw new NotImplementedException();
    }

}
