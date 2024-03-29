using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Handlers;

namespace Atlas.Core.Tokenizer.Token;

public static class TokenFactory
{
    private static IEnumerable<IHandler> handlers = new List<IHandler>
    {
        new LinkHandler(),
        new InfoboxHandler(),
        new ListHandler(),
        new ListItemHandler(),
        new SectionHandler(),
        new TableHandler(),
        new TableHeaderHandler(),
        new TableRowHandler(),
        new TextHandler(),
    };

    /// <summary>
    /// Create a wiki token based on the type of the given node. If no handler can be found for the node,
    /// returns null. This includes elements that should be filtered and container nodes.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static WikiToken? Create(INode node)
    {
        foreach (var handler in handlers)
        {
            if (handler.CanHandle(node))
            {
                return handler.Handle(node);
            }
        }
        return null;
    }
}