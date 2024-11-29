using CommandLine;
using Atlas.Console.Commands;
using Atlas.Console.Commands.WikiApi;
using Atlas.Console.Commands.Parser;

// parse arguments
await Parser.Default.ParseArguments<ParseOptions, GetPageOptions, PageContentOptions>(args)
    .MapResult(
        async (ParseOptions options) => await options.Callback(),
        async (GetPageOptions options) => await options.Callback(),
        async (PageContentOptions options) => await options.Callback(),
        (_) => Task.FromResult(1)
    );
