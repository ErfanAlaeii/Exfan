namespace ExcelCreator.Application.Tables;

public sealed class TableRowSearchCriteria
{
    public TableRowSearchTarget Target { get; init; } = TableRowSearchTarget.Column;

    public int ColumnIndex { get; init; }

    public required string SearchValue { get; init; }
}
