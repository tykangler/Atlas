using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public static class TokenFactory
{
    public static WikiToken? Create(INode node)
    {
        if (TextToken.TryParse(node) is TextToken textNode)
        {
            return textNode;
        }
        else if (SectionToken.TryParse(node) is SectionToken sectionNode)
        {
            return sectionNode;
        }
        else if (LinkToken.TryParse(node) is LinkToken linkNode)
        {
            return linkNode;
        }
        else if (ListToken.TryParse(node) is ListToken listNode)
        {
            return listNode;
        }
        else if (ListToken.TryParse(node) is ListToken listItem)
        {
            return listItem;
        }
        else if (TableToken.TryParse(node) is TableToken tableNode)
        {
            return tableNode;
        }
        else
        {
            return null;
        }
    }
}