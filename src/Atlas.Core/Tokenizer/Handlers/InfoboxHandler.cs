using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Handlers;

public class InfoboxHandler : IHandler
{
    private const string InfoboxClass = "infobox";
    private const string TableTag = "TABLE";

    public bool CanHandle(INode node)
    {
        return node is IElement element && element.IsTag(TableTag) && element.ClassList.Contains(InfoboxClass);
    }

    public WikiToken? Handle(INode node)
    {
        return null;
    }

}
