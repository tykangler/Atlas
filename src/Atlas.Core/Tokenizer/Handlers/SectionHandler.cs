using AngleSharp.Dom;
using Atlas.Core.Extensions;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class SectionHandler : IHandler
{
    private const string SectionHeadingClass = "mw-headline";

    public bool CanHandle(INode node)
    {
        return node is IElement element && element.ClassList.Contains(SectionHeadingClass);
    }

    public WikiToken? Handle(INode node)
    {
        if (!CanHandle(node))
        {
            return null;
        }
        return new SectionToken(node.Text().NormalizeWhiteSpace());
    }

}
