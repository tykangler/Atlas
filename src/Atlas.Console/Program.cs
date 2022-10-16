using CommandLine;
using Atlas.Console.Commands;
using Atlas.Console.Commands.WikiApi;

// parse arguments
await Parser.Default.ParseArguments<ExtractOptions, GetPageOptions, PageContentOptions>(args)
    .MapResult(
        async (ExtractOptions options) => await options.Callback(),
        async (GetPageOptions options) => await options.Callback(),
        async (PageContentOptions options) => await options.Callback(),
        (_) => Task.FromResult(1)
    );
