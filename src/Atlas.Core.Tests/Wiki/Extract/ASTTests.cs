using Atlas.Core.Wiki.Extract.AST;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Xunit.Abstractions;
using Atlas.Core.Tests.Utility;

namespace Atlas.Core.Tests.Wiki.Extract;

public class ASTTests
{
    private readonly Func<IElement, List<WikiNode>> testExtractFunc = elem =>
        new List<WikiNode> { new TextNode(elem.TextContent) };
    private readonly ITestOutputHelper output;

    public ASTTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Trait("Category", "Extract")]
    [Trait("AST", "Interlink")]
    [Theory]
    [InlineData(@"Test   
        Interlink")]
    [InlineData("Test        Interlink")]
    [InlineData("     Test Interlink  ")]
    [InlineData(" Test Interlink ")]
    [InlineData(@"
    Test    Interlink
    ")]
    public async Task InterLink_WhenParseSuccessful_ReturnsTrueAndNodeHasCorrectValues(string text)
    {
        var expected = "Test Interlink";
        var link = "/wiki/test-link";
        var elem = await HtmlUtility.CreateDocument(@$"
            <a href='{link}'>
                {text}
            </a>") as IElement;
        var interlinkNode = LinkNode.TryParse(elem!);
        Assert.NotNull(interlinkNode);
        Assert.True(expected == interlinkNode?.Value);
        Assert.True(link == interlinkNode?.Url);
    }

    [Trait("Category", "Extract")]
    [Fact]
    public async Task Interlink_WhenInputIsInvalid_ReturnsNullAndNodeIsNull()
    {
        var elem = await HtmlUtility.CreateDocument(
            @$"<p>Test</p>"
        ) as IElement;
        var interlinkNode = LinkNode.TryParse(elem!);
        Assert.Null(interlinkNode);
    }

    [Trait("Category", "Extract")]
    [Trait("AST", "ListNode")]
    [Fact]
    public async Task List_WhenParseSuccessful_ReturnsTrueAndNodeHasCorrectValue()
    {
        string[] expectedItems = {
            "Hello",
            "World",
            "Test",
            "List"
        };
        var unorderedList = await HtmlUtility.CreateDocument(@$"
            <ul>
                <li>{expectedItems[0]}</li>
                <li>{expectedItems[1]}</li>
                <li>{expectedItems[2]}</li>
                <li>{expectedItems[3]}</li>
            </ul>"
        ) as IElement;
        var listNode = ListNode.TryParse(unorderedList!, testExtractFunc);
        Assert.NotNull(listNode);
        Assert.True(listNode!.ListItems.Count() == 4);
        var listItems = listNode!.ListItems.ToList();
        Assert.True((listItems[0] as TextNode)?.Value == "Hello");
        Assert.True((listItems[1] as TextNode)?.Value == "World");
        Assert.True((listItems[2] as TextNode)?.Value == "Test");
        Assert.True((listItems[3] as TextNode)?.Value == "List");
    }

    [Trait("Category", "Extract")]
    [Trait("AST", "ListNode")]
    [Fact]
    public async Task List_WhenInputInvalid_ReturnsFalseAndNodeIsNull()
    {
        var unorderedList = await HtmlUtility.CreateDocument(@$"
            <a href='/wiki/test-link'>test</a>
        ") as IElement;
        var listNode = ListNode.TryParse(unorderedList!, testExtractFunc);
        Assert.Null(listNode);
    }

    [Trait("Category", "Extract")]
    [Trait("AST", "SectionNode")]
    [Fact]
    public async Task Section_WhenInputValid_ReturnsTrueAndHasCorrectValue()
    {
        var expected = "headline test";
        var section = await HtmlUtility.CreateDocument(@$"
            <h2 class='mw-headline'>   {expected} </h2>
        ") as IElement;
        var sectionNode = SectionNode.TryParse(section!);
        Assert.NotNull(sectionNode);
        Assert.True(expected == sectionNode!.Value);
    }

    [Trait("Category", "Extract")]
    [Trait("AST", "SectionNode")]
    [Fact]
    public async Task Section_WhenInputInvalid_ReturnsFalseAndNodeIsNull()
    {
        var section = await HtmlUtility.CreateDocument(@$"
            <a href='/wiki/test-link'>test</a>
        ") as IElement;
        var sectionNode = SectionNode.TryParse(section!);
        Assert.Null(sectionNode);
    }

    [Trait("Category", "Extact")]
    [Trait("AST", "Text")]
    [Fact]
    public async Task Text_WhenInputHasLiteralNewlines_NewlinesAreRemoved()
    {
        string expected = "hello";
        var text = await HtmlUtility.CreateDocument(@$"
            \n\n\n\n\n{expected}\n\n\n\n\n
        ");
        var textNode = TextNode.TryParse(text);
        Assert.True(textNode!.Value == expected);
    }

    [Trait("Category", "Extract")]
    [Trait("AST", "Text")]
    [Fact]
    public async Task Text_WhenInputHasOnlyNewlineLiterals_TextNodeIsNull()
    {
        var text = await HtmlUtility.CreateDocument(@$"
            \n\n\n\n\n\n  
            \n\n   \n\n\n\n\n
        ");
        var textNode = TextNode.TryParse(text);
        Assert.Null(textNode);
    }
}