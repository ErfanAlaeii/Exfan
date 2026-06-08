using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Tables;

public static class TableRowSearchFields
{
    public static IReadOnlyList<TableRowSearchField> FromSheet(SheetSpec sheet)
    {
        var fields = new List<TableRowSearchField>(sheet.Columns.Count + 1);
        for (var index = 0; index < sheet.Columns.Count; index++)
            fields.Add(MapColumn(sheet.Columns[index], index));

        fields.Add(new TableRowSearchField
        {
            Target = TableRowSearchTarget.CreatedAt,
            Header = TableRowMapper.TimestampColumnHeader,
            InputKind = TableRowSearchInputKind.Date
        });

        return fields;
    }

    private static TableRowSearchField MapColumn(ColumnSpec column, int index)
    {
        if (column.DropdownValues is { Count: > 0 })
        {
            return new TableRowSearchField
            {
                Target = TableRowSearchTarget.Column,
                ColumnIndex = index,
                Header = column.Header,
                InputKind = TableRowSearchInputKind.Dropdown,
                DropdownValues = column.DropdownValues
            };
        }

        if (ColumnTypes.IsDate(column.Type))
        {
            return new TableRowSearchField
            {
                Target = TableRowSearchTarget.Column,
                ColumnIndex = index,
                Header = column.Header,
                InputKind = TableRowSearchInputKind.Date
            };
        }

        return new TableRowSearchField
        {
            Target = TableRowSearchTarget.Column,
            ColumnIndex = index,
            Header = column.Header,
            InputKind = TableRowSearchInputKind.Text
        };
    }
}
