using AngleSharp.Dom;

namespace Atlas.Core.Extensions;

public static class IElementExtensions
{
    public static bool IsTag(this IElement element, string tag) => element.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase);
}
