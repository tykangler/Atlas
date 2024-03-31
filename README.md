# Atlas

## Design

### Tokenizer

Generally I'd think about the Tokenizer as converting any type of document received from the wikipedia api to a uniform tree structure, to protect against
any API changes, or if I'd like to accept a different kind of input. 

The entry point is the `ITokenizer`, which is subclassed by the `HtmlDocumentTokenizer`, and takes in an input `Document`, which is subclassed by `HtmlDocument`. 
It returns a `WikiDocument`, which contains the parsed tokens, and the original document input.

#### Implementation Details

The `ElementTokenizer` is the high level dispatch class for creating tokens. The factory `TokenFactory` registers a collection of `IHandler`, and the individual handlers do token-specific
parsing to return tokens. `ElementTokenizer` will receive all the tokens as a list (some tokens are nested, but that is handled by the handlers), and merge continuous text
tokens into a single text token, along with any other cleanup. 

A few of the handlers call `ElementTokenizer` again as they might contain fields/properties that are also `WikiToken`s. Because the `ElementTokenizer` can dispatch, and cleanup the output 
`WikiToken` results, it makes sense to call `ElementTokenizer` again to dispatch the necessary handlers to create child `WikiToken`s. Once the handler reaches its
piece of document to parse, it is now the owner of that piece and has full control over it. For example, if a `table` element comes,
and the `TableHandler` is registered for that element, then the `TableHandler` is in charge of converting that entire `table` element and any children that belong to it. 
Note that this only makes sense in a tree structure like HTML. 

If we'd like to accomodate other structures, obviously another type of handler is needed. And another `ElementTokenizer` is needed.

#### Filtering Details

Some elements are filtered out of processing, for example, citations, `sup` elements, etc. This is handled by `HtmlElementFilter`. `ElementTokenizer` will call `HtmlElementFilter.Filter` 
to filter out **all necessary elements and their children** from processing. This will happen for each recursive call. There is also top-level filtering where just the container node is filtered.
For example, `div` elements that have no role except as an element container will not be tokenized, but its **children will be**. This is done by the logic flow. If there is no handler that
can handle the element, or if the handler itself returns null (because it can't process the element), then `ElementTokenizer` will recurse down to its children and try to tokenize the children.

If element and its children must be excluded from processing, include code in the `HtmlElementFilter`. If just the container node, then it'll be filtered out by the **lack of an appropriate handler**.