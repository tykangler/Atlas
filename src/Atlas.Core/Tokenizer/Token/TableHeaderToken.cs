using AngleSharp.Dom;

namespace Atlas.Core.Tokenizer.Token;

public class TableHeaderToken : WikiToken
{
    public IEnumerable<WikiToken> TableHeaderTokens { get; }

    public override void Accept(TokenVisitor visitor)
    {
        throw new NotImplementedException();
    }


    public TableHeaderToken(IEnumerable<WikiToken> tableHeaders) => TableHeaderTokens = tableHeaders;
}
