namespace Atlas.Indexer.Extensions;

using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string NormalizeWhiteSpace(this string str) => Regex.Replace(str.Trim(), @"\s+", " ");
}
