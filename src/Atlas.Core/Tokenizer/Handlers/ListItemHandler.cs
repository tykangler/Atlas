using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class ListItemHandler : IHandler
{
    private const string ListItemTag = "LI";

    public bool CanHandle(INode node)
    {
        return node is IElement element && element.TagName == ListItemTag;
    }

    public WikiToken? Handle(INode node)
    {
        if (!(node is IElement element && CanHandle(node)))
        {
            return null;
        }
        var tokens = ElementTokenizer.TokenizeChildren(element);
        return new ListItemToken(tokens);
    }
}
