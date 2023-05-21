namespace Atlas.Core.Tokenizer.Input;

using AngleSharp.Dom;

public record HtmlDocument(string Content, INode? Node = null) : Document(Content);