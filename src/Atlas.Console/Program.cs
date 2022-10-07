using CommandLine;
using Atlas.Console.Commands;

await Parser.Default.ParseArguments<ExtractOptions, object>(args)
    .WithParsedAsync<ExtractOptions>(async options => await options.Callback());
