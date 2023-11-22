using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class ListItemToken : WikiToken
{
    private const string listTag = "LI";

    public IEnumerable<WikiToken> Tokens { get; }

    private static bool Validate(INode node)
    {
        return node is IElement element && element.TagName == listTag;
    }

    public static ListItemToken? TryParse(INode node)
    {
        if (node is IElement element && Validate(element))
        {
            var tokens = ElementTokenizer.Tokenize(element);
            return new ListItemToken(tokens);
        }
        return null;
    }

    public ListItemToken(IEnumerable<WikiToken> tokens) => Tokens = tokens;

    public override void Accept(TokenVisitor visitor) => visitor.VisitListItem(this);
}
