using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Filters;

public static class HtmlElementFilter
{
    // these will be evlauated as partial matches.
    // for example, reference-a is a match.
    private static readonly string[] disallowClasses = {
        "infobox", "reflist", "reference", "hatnote", "thumb", "noprint"
    };
    private static readonly string[] disallowTags = { "style", "sup", "img", "table", "cite" };

    public static IEnumerable<INode> FilterElements(IEnumerable<INode> nodes)
    {
        return nodes.Where(IsElementValid);
    }

    private static bool IsElementValid(INode node)
    {
        var isText = node.NodeType == NodeType.Text;
        if (isText)
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