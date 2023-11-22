namespace Atlas.Console.Commands;

using System;
using CommandLine;
using Atlas.Core.Tokenizer;
using Atlas.Console.Services;
using Atlas.Core.Services;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Clients.Wiki.Models;
using Atlas.Core.Tokenizer.Input;
using System.Diagnostics;

[Verb("parse", HelpText = "parse wikipedia html documents into token list")]
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
        using var textWriter = OutputFile == null ? Console.Out : CreateFile(OutputFile);
        var tokenizer = new HtmlTokenizer();
        IEnumerable<WikiToken>? tokens;
        if (PageTitle != null)
        {
            WikiParseResponse response;
            using (var timer = new Timer(textWriter, "Get Documents With Title"))
            {
                response = await GetWikiDocumentFromTitle(PageTitle);
            }
            using (var timer = new Timer(textWriter, "Tokenize Documents"))
            {
                tokens = await tokenizer.Tokenize(new HtmlDocument
                (
                    Content: response.Parse.Text
                ));
            }

        }
        else if (PageId >= 0)
        {
            WikiParseResponse response;
            using (var timer = new Timer(textWriter, "Get Documents With Id"))
            {
                response = await GetWikiDocumentFromId(PageId);
            }
            using (var timer = new Timer(textWriter, "Tokenize Documents"))
            {
                tokens = await tokenizer.Tokenize(new HtmlDocument
                (
                    Content: response.Parse.Text
                ));
            }
        }
        else if (Html != null)
        {
            tokens = await tokenizer.Tokenize(new HtmlDocument
            (
                Content: Html
            ));
        }
        else
        {
            Console.WriteLine("either html, title, or page-id must be specified");
            return;
        }
        TokenVisitor visitor = new WikiTokenVisitor(textWriter);
        textWriter.WriteLine("\nRESULT - ");
        foreach (var token in tokens)
        {
            token.Accept(visitor);
        }
    }

    private static async Task<WikiParseResponse> GetWikiDocumentFromId(int pageId)
    {
        var apiService = new WikiApiService(new HttpClient());
        return await apiService.ParsePageFromIdAsync(pageId.ToString());
    }

    private static async Task<WikiParseResponse> GetWikiDocumentFromTitle(string pageTitle)
    {
        var apiService = new WikiApiService(new HttpClient());
        return await apiService.ParsePageFromTitleAsync(pageTitle);
    }

    private static TextWriter CreateFile(string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return File.CreateText(path);
    }
}