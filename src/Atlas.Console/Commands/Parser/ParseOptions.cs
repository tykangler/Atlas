namespace Atlas.Console.Commands.Parser;

using System;
using Atlas.Console.Services;
using Atlas.Core.Services;
using CommandLine;
using Atlas.Console.Exceptions;
using Atlas.Core.Parser.Wiki;
using Atlas.Core.Parser;
using Atlas.Core.Model;
using Atlas.Core.Parser.Wiki.HtmlHandlers;

[Verb("parse-page", HelpText = "Parse a wiki page given either page id, page title, or page content")]
public class ParseOptions
{
    [Option('c', "content", HelpText = "Page content", SetName = "page-content", Default = null)]
    public string? Content { get; set; }

    [Option('t', "title", HelpText = "Page title", SetName = "page-title", Default = null)]
    public string? PageTitle { get; set; }

    [Option('i', "page-id", HelpText = "Page ID", SetName = "page-id", Default = null)]
    public string? PageId { get; set; }

    [Option('o', "out", HelpText = "Output file name")]
    public string? OutputFile { get; set; }

    public async Task Callback()
    {
        try
        {
            var document = await ParseContentToDocument();
            if (document == null)
            {
                Console.WriteLine("No response from API");
            }
            else
            {
                await OutputService.WriteObjectToConsoleOrFile(document, OutputFile);
            }
        }
        catch (InvalidOptionsException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task<Document?> ParseContentToDocument()
    {
        IParser parser = new WikiParser([new LinkHandler(), new SectionHandler(), new TextHandler()]);
        if (Content == null)
        {
            var apiService = new WikiApiService(new HttpClient());
            var pageContentService = new PageContentService(apiService);
            Console.WriteLine("Sending request to retrieve page");
            var pageContentResponse = await pageContentService.GetPageContent(PageTitle, PageId);
            Console.WriteLine("Retrieved page");
            if (pageContentResponse != null)
            {
                return await parser.Parse(pageContentResponse.Parse.Text);
            }
            return null;
        }
        else
        {
            return await parser.Parse(Content);
        }
    }
}
