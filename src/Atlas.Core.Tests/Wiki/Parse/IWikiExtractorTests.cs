using Atlas.Core.Wiki.Parse;
using Atlas.Core.Wiki.Parse.Token;

namespace Atlas.Core.Tests.Wiki.Parse;

public class IWikiExtractorTests
{
    [Trait("Category", "Extract")]
    [Fact]
    public async Task WhenInputIsSingleLevel_CorrectOutputIsProduced()
    {
        IWikiParser extractor = new HtmlWikiParser();
        string inputDocument = $@"
            <div class='container'> 
                <a href='/wiki/test-link'>
                    test link
                </a>
                Test text
                <div class='mw-headline'>section heading   </div>
                <ol>
                    <li>test</li>
                    <li>
                        list
                    </li>
                </ol>
            </div>
        ";
        var tokens = (await extractor.Extract(inputDocument)).ToArray();
        Assert.True(tokens.Length == 4);
        Assert.True(tokens[0] is LinkNode linkNode &&
            linkNode.Value == "test link" &&
            linkNode.Url == "/wiki/test-link" &&
            linkNode.IsInterlink);
        Assert.True(tokens[1] is TextNode textNode &&
            textNode.Value == "Test text");
        Assert.True(tokens[2] is SectionNode sectionNode &&
            sectionNode.Value == "section heading");
        var listNode = tokens[3] as ListNode;
        Assert.True(listNode != null);
        Assert.True(listNode?.ListItems.Count() == 2);
        var firstListNode = listNode!.ListItems.First() as TextNode;
        var secondListNode = listNode!.ListItems.Last() as TextNode;
        Assert.True(firstListNode != null && firstListNode.Value == "test");
        Assert.True(secondListNode != null && secondListNode.Value == "list");
    }

    [Trait("Category", "Extract")]
    [Fact]
    public async Task WhenInputHasInvalidElements_InvalidElementAndChildElementsAreSkipped()
    {
        IWikiParser extractor = new HtmlWikiParser();
        string inputDocument = @$"
            <div class='container'>
                <a href='google.com'>Hello world</a>
                Test text
                <div class='reflist'>
                    <div class='mw-references-wrap mw-references-columns'>
                        <ol class='references'>
                            <li id='cite_note-iucn-1'>test citation</li>
                        </ol>
                    </div>
                </div>     
            </div>
        ";
        var tokens = (await extractor.Extract(inputDocument)).ToArray();
        Assert.True(tokens.Length == 2);
        Assert.True(tokens[0] is LinkNode linkNode &&
                    linkNode.Value == "Hello world" &&
                    linkNode.Url == "google.com" &&
                    !linkNode.IsInterlink);
        Assert.True(tokens[1] is TextNode textNode &&
                    textNode.Value == "Test text");
    }

    [Trait("Category", "Extract")]
    [Fact]
    public async Task WhenInputHasMultipleLevels_CorrectOutputIsProduced()
    {
        IWikiParser extractor = new HtmlWikiParser();
        string inputDocument = @$"
            <div class='container'>
                <a href='/wiki/test-link'>test link</a>
                <div class='valid-class'>
                    <p>
                        test paragraph
                    </p>
                    <div>
                        <span>
                            test span 
                            <strong>
                                with strong
                            </strong>
                        </span>
                        <ul>
                            <li>list <em>with em</em></li>
                            <li>list <strong>with strong</strong></li>
                        </ul>
                    </div>
                </div>
                <h2 class='mw-headline'>Section heading</h2>
            </div>
        ";
        var tokens = (await extractor.Extract(inputDocument)).ToArray();
        Assert.True(tokens.Length == 4);
        Assert.True(tokens[0] is LinkNode linkNode &&
                    linkNode.Value == "test link" &&
                    linkNode.Url == "/wiki/test-link" &&
                    linkNode.IsInterlink);
        Assert.True(tokens[1] is TextNode textNode &&
                    textNode.Value == "test paragraph test span with strong");
        Assert.True(tokens[2] is ListNode listNode &&
                    listNode.ListItems.Count() == 2 &&
                    (listNode.ListItems.First() as TextNode)?.Value == "list with em" &&
                    (listNode.ListItems.Last() as TextNode)?.Value == "list with strong");
        Assert.True(tokens[3] is SectionNode sectionNode &&
                    sectionNode.Value == "Section heading");
    }

    [Trait("Category", "Extract")]
    [Fact]
    public async Task WhenInputHasTextElementsSandwichingInvalidElement_TextElementsAreMerged()
    {
        IWikiParser extractor = new HtmlWikiParser();
        string inputDocument = @$"
            <div class='container'>
                Hello world   
                <div class='thumb'>
                    <p>
                        test paragraph
                    </p>
                    <div>
                        <span>
                            test span 
                            <strong>
                                with strong
                            </strong>
                        </span>
                        <ul>
                            <li>list <em>with em</em></li>
                            <li>list <strong>with strong</strong></li>
                        </ul>
                    </div>
                </div>
                Hello World
            </div>
        ";
        var tokens = (await extractor.Extract(inputDocument)).ToArray();
        Assert.True(tokens.Length == 1);
        Assert.True(tokens.First() is TextNode textNode && textNode.Value == "Hello world Hello World");
    }
}