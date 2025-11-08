using System;
using System.ComponentModel.DataAnnotations;
using Atlas.Indexer.Models.Annotations;

namespace Atlas.Indexer.Models;

/// <summary>
/// Abstract base class representing a document in various stages of the indexing pipeline.
/// </summary>
/// <param name="Parsed">The parsed text content from WikiHtmlParser. This property is always populated after the parsing stage.</param>
/// <param name="Raw">The raw HTML content of the document.</param>
/// <param name="Annotations">Gets the annotations for this document. Implementations should return annotations indexed by their start character position.</param>
public record Document(string Raw, string Parsed, AnnotationCollection Annotations);
