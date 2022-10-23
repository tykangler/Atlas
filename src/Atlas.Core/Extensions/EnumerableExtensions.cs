namespace Atlas.Core.Extensions;

public static class EnumerableExtensions
{
    public static bool PartiallyContains(this IEnumerable<string> enumerable, string s, StringComparison comparisonType)
    {
        foreach (string val in enumerable)
        {
            if (val.Contains(s, comparisonType)) return true;
        }
        return false;
    }
}