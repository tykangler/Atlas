namespace Atlas.Core.Tokenizer.Token;

public abstract class WikiToken
{
    public abstract void Accept(TokenVisitor visitor);
}


// each recursive call follows the same flow
// -- check if the token matches any new node types among eligible types for the current node
// -- if it does, then call recursively for the new state
// -- Add (as children of the current node) the result of the recursive call (should be WikiNode's)
// -- If it's a terminal node, then parse the input and return a new TerminalNode with value set
//    to the parsed input

// Each node type will need an accompanying:
// -- match input to creation of the node type 
// -- determine which children are eligible for a certain node type, all eligible children will be referenced in the Parse method
// -- USE subtypes

// Parse and Match can't be declared static and abstract, just have to remember that each class will have to implement these static methods