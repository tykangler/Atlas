using AngleSharp.Dom;

namespace Atlas.Indexer.Extensions;

public static class IElementExtensions
{
    public static bool IsTag(this IElement element, string tag) => element.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase);

    public static bool HasClass(this IElement element, string targetClass, bool caseSensitive = false)
    => element.ClassList.Contains(targetClass,
        caseSensitive
        ? StringComparer.Ordinal
        : StringComparer.OrdinalIgnoreCase);
}
