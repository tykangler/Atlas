namespace Atlas.Console.Services;

using Atlas.Core.Wiki.Extract.AST;

public class WikiTokenVisitor : ASTVisitor
{
    private TextWriter writer;

    public WikiTokenVisitor(TextWriter writer) => this.writer = writer;

    public override void VisitLink(LinkNode node)
    {
        string heading = node.IsInterlink ? "INTERLINK" : "LINK";
        this.writer.WriteLine($"[{heading}] - {node.Url} - {node.Value}");
    }

    public override void VisitList(ListNode node)
    {
        this.writer.WriteLine("[LIST]");
        foreach (var item in node.ListItems)
        {
            this.writer.WriteLine($"\t{item}");
        }
    }

    public override void VisitSection(SectionNode node)
    {
        this.writer.WriteLine($"[SECTION] - {node.Value}");
    }

    public override void VisitText(TextNode node)
    {
        this.writer.WriteLine($"[TEXT] - {node.Value}");
    }

}