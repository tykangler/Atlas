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
    [Option('t', "title", HelpText = "page title of wikipedia document (mutually exclusive with page id)", SetName = "title")]
    public string? PageTitle { get; set; }

    [Option('p', "page-id", HelpText = "page id of wikipedia document (mutually exclusive with title)", Default = -1, SetName = "page-id")]
    public int PageId { get; set; }

    [Option('o', "output", HelpText = "output file")]
    public string? OutputFile { get; set; }

    public async Task Callback()
    {
        WikiParseResponse response = default!;
        if (PageTitle != null)
        {
            response = await GetWikiDocumentFromTitle(PageTitle);
        }
        else if (PageId >= 0)
        {
            response = await GetWikiDocumentFromId(PageId);
        }
        var extractor = new HtmlWikiExtractor();
        var tokens = await extractor.Extract(response.Text);
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