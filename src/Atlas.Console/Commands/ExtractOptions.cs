namespace Atlas.Console.Commands;

using System;
using CommandLine;
using Atlas.Core.Wiki.Extract;
using Atlas.Console.Services;
using Atlas.Core.Wiki.Extract.AST;

[Verb("extract", HelpText = "extract wikipedia html documents into token list")]
public class ExtractOptions
{
    [Value(0, MetaName = "html", Required = true, HelpText = "html to extract tokens from")]
    public string Html { get; set; } = string.Empty;

    public async Task Callback()
    {
        var extractor = new HtmlWikiExtractor();
        var tokens = (await extractor.Extract(this.Html));
        ASTVisitor visitor = new WikiTokenVisitor(Console.Out);
        foreach (var token in tokens)
        {
            token.Accept(visitor);
        }
    }

}