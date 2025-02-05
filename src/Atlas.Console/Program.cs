using CommandLine;
using Atlas.Console.Commands.WikiApi;
using Atlas.Console.Commands.Parser;
using Atlas.Console.Commands.Narrator;

// parse arguments
await Parser.Default.ParseArguments<ParseOptions, GetPageOptions, PageContentOptions, NarrateOptions>(args)
    .MapResult(
        async (ParseOptions options) => await options.Callback(),
        async (GetPageOptions options) => await options.Callback(),
        async (PageContentOptions options) => await options.Callback(),
        async (NarrateOptions options) => await options.Callback(),
        (_) => Task.FromResult(1)
    );
