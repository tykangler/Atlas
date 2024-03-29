using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public class LinkToken : WikiToken
{
    public string Url { get; }
    public string Value { get; }
    public bool IsInterlink { get; }

    public LinkToken(string url, string value, bool isInterlink)
    {
        Url = url;
        Value = value;
        IsInterlink = isInterlink;
    }

    public override void Accept(TokenVisitor visitor) => visitor.VisitLink(this);
}
