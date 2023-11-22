using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public static class TokenFactory
{
    public static async Task<WikiToken?> Create(INode node)
    {
        if (TextNode.TryParse(node) is TextNode textNode)
        {
            return textNode;
        }
        else if (SectionNode.TryParse(node) is SectionNode sectionNode)
        {
            return sectionNode;
        }
        else if (LinkNode.TryParse(node) is LinkNode linkNode)
        {
            return linkNode;
        }
        else if (await ListNode.TryParse(node) is ListNode listNode)
        {
            return listNode;
        }
        else if (await ListItem.TryParse(node) is ListItem listItem)
        {
            return listItem;
        }
        else if (TableNode.TryParse(node) is TableNode tableNode)
        {
            return tableNode;
        }
        else
        {
            return null;
        }
    }
}