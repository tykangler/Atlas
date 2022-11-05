using System.Text.Json;
using Atlas.Core.Wiki.Data.Models;

namespace Atlas.Core.Tests.Wiki.Data.Converters;

public class WikiPageBatchResponseConverterTests
{
    [Trait("Category", "Converters")]
    [Fact]
    public void WhenJsonIsCorrect_CorrectObjectIsReturned()
    {
        var json = @"
{
    ""batchcomplete"": """",
    ""continue"": {
        ""gapcontinue"": ""\""Baby_Lollipops\""_murder"",
        ""continue"": ""gapcontinue||""
    },
    ""query"": {
        ""pages"": {
            ""2538127"": {
                ""pageid"": 2538127,
                ""ns"": 0,
                ""title"": ""\""...And Ladies of the Club\""""
            },
            ""52243591"": {
                ""pageid"": 52243591,
                ""ns"": 0,
                ""title"": ""\""Awaken, My Love!\""""
            }
        }
    }
}";
        var converted = JsonSerializer.Deserialize<WikiPageBatchResponse>(json)!;
        Assert.NotNull(converted);
        Assert.True(converted.Continue == @"""Baby_Lollipops""_murder"); // "Baby_Lollipops"_murder
        Assert.True(converted.Pages.Count() == 2);
        Assert.True(converted.Pages.First().PageId == 2538127);
        Assert.True(converted.Pages.Last().PageId == 52243591);
    }

    [Trait("Category", "Converters")]
    [Fact]
    public void WhenOnlyErrorsPresent_CorrectJsonIsReturned()
    {
        var json = @"
{
    ""errors"": [
        {
            ""code"": ""badvalue"",
            ""text"": ""Unrecognized value for parameter \""action\"": quer."",
            ""module"": ""main""
        },
        {
            ""code"": ""ratelimit"",
            ""text"": ""Test text"",
            ""module"": ""module""
        }
    ],
    ""docref"": ""See https://en.wikipedia.org/w/api.php for API usage. Subscribe to the mediawiki-api-announce mailing list at &lt;https://lists.wikimedia.org/postorius/lists/mediawiki-api-announce.lists.wikimedia.org/&gt; for notice of API deprecations and breaking changes."",
    ""servedby"": ""mw1388""
}";
        var converted = JsonSerializer.Deserialize<WikiPageBatchResponse>(json)!;
        Assert.NotNull(converted);
        Assert.True(converted.Continue == "");
        Assert.True(!converted.Pages.Any());
        Assert.True(converted.Errors!.Count() == 2);
        Assert.True(converted.Errors!.First() == new WikiError(
            Code: "badvalue",
            Text: @"Unrecognized value for parameter ""action"": quer.",
            Module: "main"
        ));
        Assert.True(converted.Errors!.Last() == new WikiError(
            Code: "ratelimit",
            Text: "Test text",
            Module: "module"
        ));
    }

    [Trait("Category", "Converters")]
    [Fact]
    public void WhenWarningsArePresent_CorrectJsonIsReturned()
    {
        var json = @"
{
    ""errors"": [
        {
            ""code"": ""badvalue"",
            ""text"": ""Unrecognized value for parameter \""action\"": quer."",
            ""module"": ""main""
        }
    ],
    ""warnings"": [
        {
            ""code"": ""ratelimit"",
            ""text"": ""Test text"",
            ""module"": ""module""
        }
    ],
    ""batchcomplete"": """",
    ""continue"": {
        ""gapcontinue"": ""\""Baby_Lollipops\""_murder"",
        ""continue"": ""gapcontinue||""
    },
    ""query"": {
        ""pages"": [
            {
                ""pageid"": 2538127,
                ""ns"": 0,
                ""title"": ""\""...And Ladies of the Club\""""
            },
            {
                ""pageid"": 52243591,
                ""ns"": 0,
                ""title"": ""\""Awaken, My Love!\""""
            }
        ]
    }
}
";
        var converted = JsonSerializer.Deserialize<WikiPageBatchResponse>(json)!;
        Assert.NotNull(json);

        Assert.True(converted.Continue == @"""Baby_Lollipops""_murder"); // "Baby_Lollipops"_murder
        Assert.True(converted.Pages.Count() == 2);
        Assert.True(converted.Pages.First().PageId == 2538127);
        Assert.True(converted.Pages.Last().PageId == 52243591);

        Assert.True(converted.Errors!.Count() == 1);
        Assert.True(converted.Warnings!.Count() == 1);
        Assert.True(converted.Errors!.First() == new WikiError(
            Code: "badvalue",
            Text: @"Unrecognized value for parameter ""action"": quer.",
            Module: "main"
        ));
        Assert.True(converted.Warnings!.First() == new WikiError(
            Code: "ratelimit",
            Text: "Test text",
            Module: "module"
        ));
    }
}