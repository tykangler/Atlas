using CommandLine;
using Atlas.Console.Commands;
using Atlas.Console.Commands.WikiApi;

// parse arguments
await Parser.Default.ParseArguments<ExtractOptions, PageIdOptions>(args)
    .MapResult(
        async (ExtractOptions options) => await options.Callback(),
        async (PageIdOptions options) => await options.Callback(),
        (_) => Task.FromResult(1)
    );
