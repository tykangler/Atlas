using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Validators;

public static class HtmlElementValidator {
    private static readonly string[] disallowClasses = {
        "infobox", "reflist", "reference", "hatnote", "thumb", "noprint"
    };
    private static readonly string[] disallowTags = { "style", "sup", "img", "table", "cite" };

    public static bool IsElementValid(INode node) {
        var isText = node.NodeType == NodeType.Text;
        if (isText) {
            return true;
        } else if (node is IElement element) {
            return !disallowTags.Contains(element.TagName, StringComparer.OrdinalIgnoreCase) &&
            !disallowClasses.Any(cl =>
                element.ClassList.PartiallyContains(cl, StringComparison.OrdinalIgnoreCase));

        }
        return false;
    }
}