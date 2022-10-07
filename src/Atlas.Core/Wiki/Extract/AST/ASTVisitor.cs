namespace Atlas.Core.Wiki.Extract.AST;

/// <summary>
/// Traversal visitor for Wiki Abstract Syntax Tree. Defines operations 
/// allowed on the AST for each node type. 
/// </summary>
/// <remark>
/// Abstract class with virtual members so that subclasses can choose which methods to override.
/// If not overridden, then use default impl.
/// </remark>
public abstract class ASTVisitor
{
    // will be ignoring infoboxes for now, no reliable way to extract information
    // tried dbpedia + wikidata. will maybe revisit in the future
    // but even then, there's some irrelevant and unclean information, random links to nowhere. 
    // public virtual void VisitTable(TableNode node) { }
    public virtual void VisitLink(LinkNode node) { }
    public virtual void VisitText(TextNode node) { }
    public virtual void VisitSection(SectionNode node) { }
    public virtual void VisitList(ListNode node) { }
}