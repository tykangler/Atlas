using AngleSharp.Dom;
using Atlas.Core.Tokenizer.Models;

namespace Atlas.Core.Tokenizer.Token;

public class ListItem : WikiToken
{
    private const string listTag = "LI";

    public IEnumerable<WikiToken> Tokens { get; }

    private static bool Validate(INode node)
    {
        return node is IElement element && element.TagName == listTag;
    }

    public static async Task<ListItem?> TryParse(INode node)
    {
        if (node is IElement element && Validate(element))
        {
            var tokens = await ElementTokenizer.TokenizeElement(element);
            return new ListItem(tokens);
        }
        return null;
    }

    public ListItem(IEnumerable<WikiToken> tokens) => Tokens = tokens;

    public override void Accept(TokenVisitor visitor) => visitor.VisitListItem(this);
}
