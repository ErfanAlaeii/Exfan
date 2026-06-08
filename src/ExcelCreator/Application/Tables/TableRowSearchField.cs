namespace ExcelCreator.Application.Tables;

public enum TableRowSearchTarget
{
    Column,
    CreatedAt
}

public enum TableRowSearchInputKind
{
    Text,
    Date,
    Dropdown
}

public sealed class TableRowSearchField
{
    public TableRowSearchTarget Target { get; init; }

    public int ColumnIndex { get; init; }

    public required string Header { get; init; }

    public TableRowSearchInputKind InputKind { get; init; }

    public IReadOnlyList<string>? DropdownValues { get; init; }
}
