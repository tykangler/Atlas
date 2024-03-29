namespace Atlas.Core.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Returns true if the enumerable of strings contains a string where the string contains substring s.
    /// </summary>
    /// <param name="enumerable"></param>
    /// <param name="s"></param>
    /// <param name="comparisonType"></param>
    /// <returns></returns>
    public static bool PartiallyContains(this IEnumerable<string> enumerable, string s, StringComparison comparisonType)
    {
        foreach (string val in enumerable)
        {
            if (val.Contains(s, comparisonType)) return true;
        }
        return false;
    }
}