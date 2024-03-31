namespace Atlas.Console.Services;

using System.Runtime.CompilerServices;
using Atlas.Core.Tokenizer.Token;

public class WikiTokenVisitor : TokenVisitor
{
    private readonly TextWriter writer;

    public WikiTokenVisitor(TextWriter writer) => this.writer = writer;

    public override void VisitLink(LinkToken node)
    {
        string heading = node.IsInterlink ? "INTERLINK" : "LINK";
        writer.WriteLine($"[{heading}/] - {node.Url} - {node.Value}");
    }

    public override void VisitList(ListToken node)
    {
        writer.WriteLine($"[LIST ({node.ListType})]");
        foreach (var item in node.ListItems)
        {
            item.Accept(this);
        }
        writer.WriteLine("[/LIST]");
    }

    public override void VisitListItem(ListItemToken node)
    {
        foreach (var item in node.Tokens)
        {
            writer.WriteLine($"[LIST ITEM]");
            item.Accept(this);
            writer.WriteLine($"[/LIST ITEM]");
        }
    }

    public override void VisitSection(SectionToken node)
    {
        writer.WriteLine($"[SECTION/] - {node.Value}");
    }

    public override void VisitText(TextToken node)
    {
        writer.WriteLine($"[TEXT/] - {node.Value}");
    }

    public override void VisitTable(TableToken node)
    {
        writer.WriteLine($"[TABLE]");
        writer.WriteLine($"[HEADERS]");
        foreach (var header in node.Headers.GetHeaders())
        {
            writer.WriteLine($"[TABLE HEADER(ColSpan={header.ColSpan})]");
            foreach (var token in header.Tokens)
            {
                token.Accept(this);
            }
            writer.WriteLine("[/TABLE HEADER]");
        }
        writer.WriteLine($"[/HEADERS]");
        writer.WriteLine($"[DATA]");
        foreach (var data in node.Data)
        {
            writer.WriteLine("[DATAROW]");
            foreach (var cell in data.RowData)
            {
                writer.WriteLine("[DATACELL]");
                foreach (var token in cell.CellData)
                {
                    token.Accept(this);
                }
                writer.WriteLine("[/DATACELL]");
            }
            writer.WriteLine("[/DATAROW]");
        }
        writer.WriteLine($"[/DATA]");
        writer.WriteLine("[/TABLE]");
    }
}