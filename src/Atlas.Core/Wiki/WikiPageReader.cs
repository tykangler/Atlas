// call api query all pages with max limit, then call api parse for each page. Delay of 0.1 ms per page
// should run in parallel with the parser 

namespace Atlas.Core.Wiki;

public class WikiPageReader : IWikiReader
{
    // GetPages() =>
    //      await get pages
    //      foreach page
    //          await parse page
    //          yield return page
    // GetPages()
    // extractor.Extract(pages.Next)
    // event-based pattern
    // getting pages is the main loop
    // when new page is parsed -> enqueue a task to extract the parsed page into wikinodes
    public void Foo()
    {
    }
}