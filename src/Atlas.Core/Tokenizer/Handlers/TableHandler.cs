using System.Data;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Atlas.Core.Extensions;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Tokenizer.Token.Table;

namespace Atlas.Core.Tokenizer.Handlers;

public class TableHandler : IHandler
{
    private const string TableTag = "TABLE";
    private const string TableHeaderTag = "TH";
    private const string TableDataTag = "TD";

    public bool CanHandle(INode node)
    {
        return node is IElement element && element.IsTag(TableTag);
    }

    public WikiToken? Handle(INode node)
    {
        if (!(node is IElement element && CanHandle(node)))
        {
            return null;
        }
        var tableElement = (element as IHtmlTableElement)!;
        var tableHeaderRowElements = GetTableHeaderRows(tableElement);
        var tableHeaders = TokenizeTableHeaders(tableHeaderRowElements);
        var tableDataRowElements = GetTableDataRows(tableElement);
        var tableData = TokenizeTableDataRows(tableDataRowElements);
        return new TableToken(
            Headers: new TableHeaderRow(tableHeaders),
            Data: tableData
        );
    }

    /// <summary>
    /// Gets a list of row elements (TR) that can be interpreted as header rows. 
    /// </summary>
    /// <param name="tableElement"></param>
    /// <returns></returns>
    private IEnumerable<IHtmlTableRowElement> GetTableHeaderRows(IHtmlTableElement tableElement)
    {
        var tableBodies = tableElement.Bodies;
        var tableHeadHeaderRows = tableElement.Head?.Rows
            ?? Enumerable.Empty<IHtmlTableRowElement>();
        if (!tableHeadHeaderRows.Any())
        {
            // is there a need to handle each section differently?
            var tableBodyHeaderRows = tableBodies
                ?.SelectMany(body => body.Rows)
                ?.Where(TableBodyRowHasHeaders)
                ?? Enumerable.Empty<IHtmlTableRowElement>();
            return tableBodyHeaderRows;
        }
        return tableHeadHeaderRows;
    }

    /// <summary>
    /// Gets a list of row elements (TR) that can be interpreted as data rows.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private IEnumerable<IHtmlTableRowElement> GetTableDataRows(IHtmlTableElement element)
    {
        var tableBodies = element.Bodies;
        var tableDataRows = tableBodies
            ?.SelectMany(body => body.Rows)
            ?.Where(TableBodyRowHasData)
            ?? Enumerable.Empty<IHtmlTableRowElement>();
        return tableDataRows;
    }

    /// <summary>
    /// Builds a list of TableHeader nodes that can be traversed as a tree,
    /// given the list of table rows (TR) that could have headers (TH) in them.
    /// Only the top level nodes are returned as child headers are stored within the table header object.
    /// </summary>
    /// <param name="headerRows"></param>
    /// <returns></returns>
    private IEnumerable<TableHeader> TokenizeTableHeaders(IEnumerable<IHtmlTableRowElement> headerRows)
    {
        var currentRow = headerRows.FirstOrDefault();
        if (currentRow == null)
        {
            return Enumerable.Empty<TableHeader>();
        }
        var headerElements = currentRow.Cells.Where(child => child.IsTag(TableHeaderTag));
        if (!headerElements.Any())
        {
            return Enumerable.Empty<TableHeader>();
        }
        return headerElements.Select(headerElem =>
        {
            var colSpan = headerElem.ColumnSpan;
            var childHeadersLocations = headerRows.Skip(headerElem.RowSpan);
            var childHeaders = TokenizeTableHeaders(childHeadersLocations);
            var headerTokens = ElementTokenizer.TokenizeChildren(headerElem);
            return new TableHeader(headerTokens, colSpan, childHeaders);
        });
    }

    /// <summary>
    /// Tokenize a collection of data rows (TR) to a collection of <see cref="TableDataRow"/> tokens.
    /// </summary>
    /// <param name="dataRows"></param>
    /// <returns></returns>
    private IEnumerable<TableDataRow> TokenizeTableDataRows(IEnumerable<IHtmlTableRowElement> dataRows)
        => dataRows.Select(row => new TableDataRow(TokenizeTableData(row)));

    /// <summary>
    /// Tokenize a single data row into a list of <see cref="TableData"/> tokens.
    /// </summary>
    /// <param name="dataRow"></param>
    /// <returns></returns>
    private IEnumerable<TableData> TokenizeTableData(IHtmlTableRowElement dataRow)
        => dataRow.Cells.Select(cell => new TableData(
            ElementTokenizer.TokenizeChildren(cell), cell.RowSpan, cell.ColumnSpan));

    /// <summary>
    /// Returns true if the table row element has any header elements (TH) as children.
    /// </summary>
    /// <param name="tableBodyRow"></param>
    /// <returns></returns>
    private bool TableBodyRowHasHeaders(IHtmlTableRowElement tableBodyRow)
    => tableBodyRow.Cells.Any(child => child.IsTag(TableHeaderTag));

    /// <summary>
    /// Returns true if the table row element (TR) has any data elements (TD) as children
    /// </summary>
    /// <param name="tableBodyRow"></param>
    /// <returns></returns>
    private bool TableBodyRowHasData(IHtmlTableRowElement tableBodyRow)
    => tableBodyRow.Cells.Any(child => child.IsTag(TableDataTag));
}
