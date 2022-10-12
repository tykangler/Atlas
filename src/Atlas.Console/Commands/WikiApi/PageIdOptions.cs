namespace Atlas.Console.Commands.WikiApi;

using System;
using System.Diagnostics;
using CommandLine;
using Atlas.Core.Wiki.Data;
using Atlas.Core.Exceptions;

[Verb("wiki-page-ids", HelpText = "Get Wikipedia page ids")]
public class PageIdOptions
{
    [Option('t', "timeout", HelpText = "timeout in milliseconds before cancelling operation", Required = true)]
    public int Timeout { get; set; }

    [Option('o', "out", HelpText = "output file to print page ids. prints to console if not specified.")]
    public string? OutFile { get; set; }

    [Option('c', "chunk-size", HelpText = "only valid with --output specified. Writes chunk-size page ids per line.")]
    public int ChunkSize { get; set; }

    public async Task Callback()
    {
        var apiService = new WikiApiService(new HttpClient());
        var stopWatch = Stopwatch.StartNew();
        List<uint> allPageIds = new List<uint>();
        try
        {
            using (var tokenSource = new CancellationTokenSource(millisecondsDelay: this.Timeout))
            {
                await foreach (var pageId in apiService.GetPageIds().WithCancellation(tokenSource.Token))
                {
                    allPageIds.Add(pageId);
                }
            }
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
        catch (OperationCanceledException)
        {
            stopWatch.Stop();
            if (OutFile != null)
            {
                var chunkedResult = allPageIds.Chunk(ChunkSize).Select(chunk => string.Join(", ", chunk));
                await File.WriteAllLinesAsync(OutFile, chunkedResult);
            }
            else
            {
                foreach (uint id in allPageIds)
                {
                    Console.WriteLine(id);
                }
            }
            Console.WriteLine("operation canceled");
            Console.WriteLine($"{stopWatch.ElapsedMilliseconds / 1000.0} seconds elapsed");
            Console.WriteLine($"{allPageIds.Count} page ids retrieved");
        }
    }

}