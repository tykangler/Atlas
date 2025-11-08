namespace Atlas.Console.Commands.WikiApi;

using System;
using Atlas.Console.Services;
using Atlas.Clients.Wiki;
using CommandLine;
using Atlas.Console.Exceptions;

[Verb("wiki-page-content", HelpText = "Parse a page given a page id, or title")]
public class PageContentOptions
{
    [Option('p', "page-id", HelpText = "page id (mutually exclusive with page title)", SetName = "page-id", Default = null)]
    public string? PageId { get; set; }

    [Option('t', "title", HelpText = "page title (mutually exclusive with page id)", SetName = "title", Default = null)]
    public string? Title { get; set; }

    [Option('h', "hidden", HelpText = "display hidden categories", Default = false)]
    public bool DisplayHidden { get; set; }

    [Option('o', "out", HelpText = "output file")]
    public string? OutputFile { get; set; }

    public async Task Callback()
    {
        var apiService = new WikiService(new HttpClient());
        var pageContentService = new PageContentService(apiService);
        try
        {
            Console.WriteLine("Sending request to retrieve page");
            var response = await pageContentService.GetPageContent(Title, PageId);
            Console.WriteLine("Retrieved page");
            if (response != null)
            {
                await OutputService.WriteObjectToConsoleOrFile(response, OutputFile);
            }
            else
            {
                Console.WriteLine("No response returned from API");
            }
        }
        catch (InvalidOptionsException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}