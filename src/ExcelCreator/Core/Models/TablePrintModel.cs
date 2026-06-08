namespace ExcelCreator.Core.Models;

public sealed class TablePrintModel
{
    public required string Title { get; init; }

    public required IReadOnlyList<string> Headers { get; init; }

    public required IReadOnlyList<IReadOnlyList<string>> Rows { get; init; }
}
