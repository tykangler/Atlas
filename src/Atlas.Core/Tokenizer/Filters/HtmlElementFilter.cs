using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Filters;

public static class HtmlElementFilter
{
    // these will be evaluated as partial matches.
    // for example, reference-a is a match.
    private static readonly string[] disallowClasses = {
        "infobox", "reflist", "reference", "hatnote", "thumb", "noprint"
    };
    private static readonly string[] disallowTags = { "style", "sup", "img", "cite", "script", "table" };

    public static IEnumerable<INode> Filter(IEnumerable<INode> nodes)
    {
        return nodes.Where(IsElementValid);
    }

    public static bool IsElementValid(INode node)
    {
        if (node.NodeType == NodeType.Text)
        {
            return true;
        }
        else if (node is IElement element)
        {
            var isValidTag = !disallowTags.Contains(element.TagName, StringComparer.OrdinalIgnoreCase);
            var doesNotHaveInvalidClasses = !disallowClasses.Any(cl =>
                element.ClassList.PartiallyContains(cl, StringComparison.OrdinalIgnoreCase));
            return isValidTag && doesNotHaveInvalidClasses;
        }
        return false;
    }
}