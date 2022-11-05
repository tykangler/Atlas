namespace Atlas.Console.Commands.WikiApi;

using System;
using System.Diagnostics;
using CommandLine;
using Atlas.Core.Wiki.Data;
using Atlas.Core.Exceptions;
using Atlas.Core.Wiki.Data.Models;
using Atlas.Console.Services;

[Verb("wiki-get-page", HelpText = "Get Wikipedia page ids")]
public class GetPageOptions
{
    [Option('t', "timeout", HelpText = "timeout in milliseconds before cancelling operation", Required = true, Default = 1000)]
    public int Timeout { get; set; }

    [Option('o', "out", HelpText = "output file to print page ids. prints to console if not specified.")]
    public string? OutFile { get; set; }

    [Option('c', "chunk-size", HelpText = "only valid with --output specified. Writes chunk-size pages per line.", Default = 1)]
    public int ChunkSize { get; set; }

    [Option('p', "page-titles", HelpText = "return page titles instead of page ids", Default = false)]
    public bool PrintTitles { get; set; }

    public async Task Callback()
    {
        var apiService = new WikiApiService(new HttpClient());
        List<WikiPage> allPages = new();
        List<WikiError> warnings = new();
        var stopWatch = Stopwatch.StartNew();
        try
        {
            string continueFrom = "";
            while (stopWatch.ElapsedMilliseconds < Timeout)
            {
                var pageBatch = await apiService.GetPages(continueFrom);
                continueFrom = pageBatch.Continue.GapContinue;
                allPages.AddRange(pageBatch.Query.Pages);
                if (pageBatch.Warnings != null)
                {
                    warnings.AddRange(pageBatch.Warnings); // only warnings since actual errors will throw exception
                }
            }
            stopWatch.Stop();
            await OutputResult(allPages, warnings, stopWatch.ElapsedMilliseconds);
        }
        catch (WikiApiException ex)
        {
            Console.WriteLine(ex.Message);
            if (ex.Errors != null)
            {
                foreach (var error in ex.Errors)
                {
                    Console.WriteLine(error);
                }
            }
        }
    }

    private async Task OutputResult(List<WikiPage> allPages, List<WikiError> warnings, long elapsedMilliseconds)
    {
        if (OutFile != null)
        {
            var chunkedResult = allPages
                .Select(page => PrintTitles ? page.Title : page.PageId.ToString())
                .Chunk(ChunkSize)
                .Select(chunk => string.Join(", ", chunk));
            using var outStream = FileUtilities.CreateFile(OutFile);
            await PrintLines(outStream, chunkedResult);
        }
        else
        {
            await PrintLines(Console.Out, allPages.Select(page =>
                PrintTitles ? page.Title : page.PageId.ToString()));
        }
        Console.WriteLine($"{elapsedMilliseconds / 1000.0} seconds elapsed");
        Console.WriteLine($"{allPages.Count()} pages retrieved");
    }

    private async Task PrintLines(TextWriter writer, IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            await writer.WriteLineAsync(line);
        }
    }
}