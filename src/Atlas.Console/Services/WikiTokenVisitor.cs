namespace Atlas.Console.Services;

using Atlas.Core.Tokenizer.Token;

public class WikiTokenVisitor : TokenVisitor
{
    private readonly TextWriter writer;

    public WikiTokenVisitor(TextWriter writer) => this.writer = writer;

    public override void VisitLink(LinkToken node)
    {
        string heading = node.IsInterlink ? "INTERLINK" : "LINK";
        writer.WriteLine($"[{heading}] - {node.Url} - {node.Value}");
    }

    public override void VisitList(ListToken node)
    {
        writer.WriteLine($"[LIST ({node.ListType})]");
        foreach (var item in node.ListItems)
        {
            item.Accept(this);
        }
    }

    public override void VisitListItem(ListToken node)
    {
        foreach (var item in node.Tokens)
        {
            writer.WriteLine($"[LIST ITEM]");
            item.Accept(this);
        }
    }

    public override void VisitSection(SectionToken node)
    {
        writer.WriteLine($"[SECTION] - {node.Value}");
    }

    public override void VisitText(TextToken node)
    {
        writer.WriteLine($"[TEXT] - {node.Value}");
    }

}