using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public static class TokenFactory
{
    public static WikiToken? Create(INode node)
    {
        if (TextToken.TryParse(node) is TextToken textToken)
        {
            return textToken;
        }
        else if (SectionToken.TryParse(node) is SectionToken sectionToken)
        {
            return sectionToken;
        }
        else if (LinkToken.TryParse(node) is LinkToken linkToken)
        {
            return linkToken;
        }
        else if (ListToken.TryParse(node) is ListToken listToken)
        {
            return listToken;
        }
        else if (ListItemToken.TryParse(node) is ListItemToken listItemToken)
        {
            return listItemToken;
        }
        else if (TableToken.TryParse(node) is TableToken tableToken)
        {
            return tableToken;
        }
        return null;
    }
}