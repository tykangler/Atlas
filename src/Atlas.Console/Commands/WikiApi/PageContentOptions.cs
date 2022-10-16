namespace Atlas.Console.Commands.WikiApi;

using System;
using System.Text.Json;
using Atlas.Core.Wiki.Data;
using Atlas.Core.Wiki.Data.Models;
using CommandLine;

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
        WikiParseResponse response = default!;
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

    private async Task WriteToFile(WikiParseResponse response, string fileName)
    {
        using var outStream = CreateFile(fileName);
        await outStream.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
    }

    private void WriteToConsole(WikiParseResponse response)
    {
        Console.WriteLine($"Found page {response.Title}");
        if (response.Redirects.Count() > 0)
        {
            Console.WriteLine($"Page with id {PageId} is a redirect page");
            foreach (var redirect in response.Redirects)
            {
                Console.WriteLine($"\tFrom: {redirect.From}, To: {redirect.To}");
            }
        }
        Console.WriteLine("============================");
        Console.WriteLine($"Title: {response.Title}");
        Console.WriteLine("Categories: ");
        foreach (var category in response.Categories)
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
        Console.WriteLine(response.Text);
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