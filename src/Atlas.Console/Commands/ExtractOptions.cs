namespace Atlas.Console.Commands;

using System;
using CommandLine;
using Atlas.Core.Wiki.Extract;
using Atlas.Console.Services;
using Atlas.Core.Wiki.Extract.AST;
using Atlas.Core.Wiki.Data;
using Atlas.Core.Wiki.Data.Models;

[Verb("extract", HelpText = "extract wikipedia html documents into token list")]
public class ExtractOptions
{
    [Option('t', "title", HelpText = "page title of wikipedia document (mutually exclusive with page id and html)", SetName = "title")]
    public string? PageTitle { get; set; }

    [Option('p', "page-id", HelpText = "page id of wikipedia document (mutually exclusive with title and html)", Default = -1, SetName = "page-id")]
    public int PageId { get; set; }

    [Option('o', "output", HelpText = "output file")]
    public string? OutputFile { get; set; }

    [Option('h', "html", HelpText = "html (mutually exclusive with title and page id)", SetName = "html")]
    public string? Html { get; set; }

    public async Task Callback()
    {
        var extractor = new HtmlWikiExtractor();
        var tokens = Enumerable.Empty<WikiNode>();
        if (PageTitle != null)
        {
            WikiParseResponse response = await GetWikiDocumentFromTitle(PageTitle);
            tokens = await extractor.Extract(response.Parse.Text);
        }
        else if (PageId >= 0)
        {
            WikiParseResponse response = await GetWikiDocumentFromId(PageId);
            tokens = await extractor.Extract(response.Parse.Text);
        }
        else if (Html != null)
        {
            tokens = await extractor.Extract(Html);
        }
        else
        {
            Console.WriteLine("either html, title, or page-id must be specified");
            return;
        }
        using var textWriter = OutputFile == null ? Console.Out : CreateFile(OutputFile);
        ASTVisitor visitor = new WikiTokenVisitor(textWriter);
        foreach (var token in tokens)
        {
            token.Accept(visitor);
        }
    }

    private async Task<WikiParseResponse> GetWikiDocumentFromId(int pageId)
    {
        var apiService = new WikiApiService(new HttpClient());
        return await apiService.ParsePageFromIdAsync(pageId.ToString());
    }

    private async Task<WikiParseResponse> GetWikiDocumentFromTitle(string pageTitle)
    {
        var apiService = new WikiApiService(new HttpClient());
        return await apiService.ParsePageFromTitleAsync(pageTitle);
    }

    private TextWriter CreateFile(string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return File.CreateText(path);
    }
}