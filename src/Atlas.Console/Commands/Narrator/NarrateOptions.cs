namespace Atlas.Console.Commands.Narrator;

using System;
using Atlas.Console.Services;
using CommandLine;
using Atlas.Console.Exceptions;
using Atlas.Core.Clients.Narrator.Models;
using System.Text.Json;
using Atlas.Core.Model;
using Atlas.Core.Parser;
using Atlas.Core.Parser.Wiki;
using Atlas.Core.Parser.Wiki.HtmlHandlers;
using Atlas.Core.Services;
using Atlas.Core.Clients.Narrator;
using static Atlas.Core.Clients.Generated.Narrator;
using Grpc.Net.Client;
using System.Diagnostics;

[Verb("narrate", HelpText = "Narrate a piece of text or a wiki page")]
public class NarrateOptions
{
    [Option('u', "url", HelpText = "Narrator Service URL", Default = "http://localhost:50051")]
    public string? NarratorUrl { get; set; }

    [Option('f', "file", HelpText = "Input file with single json representing DocumentRequest object", SetName = "content", Default = null)]
    public string? DocumentRequestFile { get; set; }

    [Option('t', "title", HelpText = "Page title", SetName = "page-title", Default = null)]
    public string? PageTitle { get; set; }

    [Option('i', "page-id", HelpText = "Page ID", SetName = "page-id", Default = null)]
    public string? PageId { get; set; }

    [Option('o', "out", HelpText = "Output file name", Default = null)]
    public string? OutputFile { get; set; }

    public async Task Callback()
    {
        try
        {
            IEnumerable<RelationshipResponse> responses = Enumerable.Empty<RelationshipResponse>();
            if (DocumentRequestFile != null)
            {
                using var fileStream = File.OpenRead(DocumentRequestFile);
                var documentRequest = SerializationService.Deserialize<DocumentRequest>(fileStream) ?? new DocumentRequest(string.Empty, []);
                Console.WriteLine($"Received document request");
                Console.WriteLine($"\tText: {documentRequest.Text}");
                Console.WriteLine($"\tPhrases: {string.Join(',', documentRequest.TargetPhrases.Select(p => p.Text))}");

                var watch = Stopwatch.StartNew();
                INarratorService client = CreateNarratorService();
                watch.Stop();
                Console.WriteLine($"Initialized client ({watch.ElapsedMilliseconds} ms)...");
                Console.WriteLine($"Calling narrator service at {NarratorUrl}...");
                await foreach (var response in client.GetRelationships(documentRequest))
                {
                    responses = responses.Append(response);
                }
            }
            if (OutputFile != null)
            {
                Console.WriteLine($"Writing to {OutputFile}");
            }
            await OutputService.WriteObjectToConsoleOrFile(responses, OutputFile);
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (InvalidOptionsException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task<Document?> ParseContentToDocument()
    {
        IParser parser = new WikiHtmlParser(WikiHandlerFactory.Default);
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

    private INarratorService CreateNarratorService()
    {
        var client = CreateClient();
        return new GrpcNarratorService(client);
    }

    private NarratorClient CreateClient()
    {
        var channel = CreateChannel();
        return new NarratorClient(channel);
    }

    private GrpcChannel CreateChannel()
    {
        return GrpcChannel.ForAddress(NarratorUrl!);
    }
}
