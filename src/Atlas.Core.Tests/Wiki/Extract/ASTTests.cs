using Atlas.Core.Wiki.Extract.AST;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Xunit.Abstractions;
using Atlas.Core.Tests.Utility;

namespace Atlas.Core.Tests.Wiki.Extract;

public class ASTTests
{

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
        bool successful = LinkNode.TryParse(elem!, out var interlinkNode);
        Assert.True(successful);
        Assert.NotNull(interlinkNode);
        Assert.True(expected == interlinkNode!.Value);
        Assert.True(link == interlinkNode!.Url);
    }

    [Trait("Category", "Extract")]
    [Fact]
    public async Task Interlink_WhenInputIsInvalid_ReturnsNullAndNodeIsNull()
    {
        var elem = await HtmlUtility.CreateDocument(
            @$"<p>Test</p>"
        ) as IElement;
        bool successful = LinkNode.TryParse(elem!, out var interlinkNode);
        Assert.False(successful);
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
                <li>
                    {expectedItems[0]}
                </li>
                <li>
                    {expectedItems[1]}
                </li>
                <li>
                    {expectedItems[2]}
                </li>
                <li>
                    {expectedItems[3]}
                </li>
            </ul>"
        ) as IElement;
        bool successful = ListNode.TryParse(unorderedList!, out var listNode);
        Assert.True(successful);
        Assert.NotNull(listNode);
        Assert.True(expectedItems.All(listNode!.ListItems.Contains));
    }

    [Trait("Category", "Extract")]
    [Trait("AST", "ListNode")]
    [Fact]
    public async Task List_WhenInputInvalid_ReturnsFalseAndNodeIsNull()
    {
        var unorderedList = await HtmlUtility.CreateDocument(@$"
            <a href='/wiki/test-link'>test</a>
        ") as IElement;
        bool successful = ListNode.TryParse(unorderedList!, out var listNode);
        Assert.False(successful);
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
        bool successful = SectionNode.TryParse(section!, out var sectionNode);
        Assert.True(successful);
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
        bool successful = SectionNode.TryParse(section!, out var sectionNode);
        Assert.False(successful);
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
        bool successful = TextNode.TryParse(text, out var textNode);
        Assert.True(successful);
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
        bool successful = TextNode.TryParse(text, out var textNode);
        Assert.False(successful);
        Assert.Null(textNode);
    }
}