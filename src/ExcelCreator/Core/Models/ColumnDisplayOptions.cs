namespace ExcelCreator.Core.Models;

public static class ColumnDisplayOptions
{
    public static bool IsMultiline(ColumnSpec column) => column.Multiline;

    public static bool SheetHasMultilineColumns(SheetSpec sheet) =>
        sheet.Columns.Any(IsMultiline);
}
