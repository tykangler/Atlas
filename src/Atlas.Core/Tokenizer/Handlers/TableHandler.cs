using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class TableHandler : IHandler
{
    private const string TableTokenTag = "TABLE";
    private const string TableHeaderTag = "TH";
    private const string TableTokenRowTag = "TR";
    private const string TableTokenDataTag = "TD";

    public bool CanHandle(INode node)
    {
        return false;
        // return node is IElement element && element.TagName == "table";
    }

    public WikiToken? Handle(INode node)
    {
        // default implementation 
        if (!(node is IElement element && CanHandle(node)))
        {
            return null;
        }
        var tableHeaderElements = element.Children.Where(child => child.TagName == TableHeaderTag);
        var tableHeaders = ElementTokenizer.TokenizeNodes(tableHeaderElements);
        return null;
    }

}
