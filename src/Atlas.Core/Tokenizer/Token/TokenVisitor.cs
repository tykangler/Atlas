namespace Atlas.Core.Tokenizer.Token;

/// <summary>
/// Traversal visitor for Wiki Token Tree. Defines operations 
/// allowed on the tree for each node type. 
/// </summary>
/// <remark>
/// Abstract class with virtual members so that subclasses can choose which methods to override.
/// If not overridden, then use default impl.
/// </remark>
public abstract class TokenVisitor
{
    public virtual void VisitInfobox(InfoboxToken node) { }
    public virtual void VisitLink(LinkToken node) { }
    public virtual void VisitText(TextToken node) { }
    public virtual void VisitSection(SectionToken node) { }
    public virtual void VisitList(ListToken node) { }
    public virtual void VisitListItem(ListItemToken node) { }
    public virtual void VisitTable(TableToken node) { }
}