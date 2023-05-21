namespace Atlas.Console.Commands.WikiApi;

using System;
using System.Text.Json;
using Atlas.Console.Services;
using Atlas.Core.Services;
using CommandLine;
using Atlas.Core.Clients.Wiki.Models;

[Verb("wiki-page-content", HelpText = "Parse a page given a page id")]
public class PageContentOptions
{
    [Option('p', "page-id", HelpText = "page id (mutually exclusive with page title)", SetName = "page-id", Default = -1)]
    public int PageId { get; set; }

    [Option('t', "title", HelpText = "page title (mutually exclusive with page id)", SetName = "title")]
    public string? Title { get; set; }

    [Option('h', "hidden", HelpText = "display hidden categories", Default = false)]
    public bool DisplayHidden { get; set; }

    [Option('o', "output", HelpText = "output file")]
    public string? OutputFile { get; set; }

    public async Task Callback()
    {
        var apiService = new WikiApiService(new HttpClient());
        WikiParseResponse response;
        if (PageId >= 0)
        {
            response = await apiService.ParsePageFromIdAsync(PageId.ToString());
            await WriteResponse(response);
        }
        else if (Title != null)
        {
            response = await apiService.ParsePageFromTitleAsync(Title);
            await WriteResponse(response);
        }
        else
        {
            Console.WriteLine("At least page-id or title must be specified");
        }

    }

    private async Task WriteResponse(WikiParseResponse response)
    {
        if (OutputFile == null)
        {
            WriteToConsole(response);
        }
        else
        {
            await WriteToFile(response, OutputFile);
        }
    }

    private static async Task WriteToFile(WikiParseResponse response, string fileName)
    {
        using var outStream = FileUtilities.CreateFile(fileName);
        await outStream.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
    }

    private void WriteToConsole(WikiParseResponse response)
    {
        Console.WriteLine($"Found page {response.Parse.Title}");
        if (response.Parse.Redirects.Any())
        {
            Console.WriteLine($"Page with id {PageId} is a redirect page");
            foreach (var redirect in response.Parse.Redirects)
            {
                Console.WriteLine($"\tFrom: {redirect.From}, To: {redirect.To}");
            }
        }
        Console.WriteLine("============================");
        Console.WriteLine($"Title: {response.Parse.Title}");
        Console.WriteLine("Categories: ");
        foreach (var category in response.Parse.Categories)
        {
            if (category.Hidden && DisplayHidden)
            {
                Console.WriteLine("\t" + category.Category);
            }
            else if (!category.Hidden)
            {
                Console.WriteLine("\t" + category.Category);
            }
        }
        Console.WriteLine("Text: ");
        Console.WriteLine(response.Parse.Text);
    }
}