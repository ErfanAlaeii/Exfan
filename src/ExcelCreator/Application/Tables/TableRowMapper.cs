using ExcelCreator.Application.Common;
using ExcelCreator.Localization;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Tables;

public static class TableRowMapper
{
    public static string TimestampColumnHeader => PersianStrings.RowTimestampColumnHeader;

    public static TemplateDefinition WithTimestampColumn(TemplateDefinition template)
    {
        var sourceSheet = template.RequirePrimarySheet();
        var exportColumns = sourceSheet.Columns
            .Select(column => new ColumnSpec
            {
                Header = column.Header,
                Type = column.Type,
                Width = column.Width,
                Formula = column.Formula,
                DropdownValues = column.DropdownValues?.ToList(),
                DropdownSource = column.DropdownSource,
                Multiline = column.Multiline
            })
            .ToList();

        exportColumns.Add(new ColumnSpec
        {
            Header = TimestampColumnHeader,
            Type = ColumnTypes.Text,
            Width = 18
        });

        return new TemplateDefinition
        {
            Id = template.Id,
            Version = template.Version,
            Title = template.Title,
            Description = template.Description,
            Category = template.Category,
            Icon = template.Icon,
            DefaultFileName = template.DefaultFileName,
            Workbook = new WorkbookSpec
            {
                Sheets =
                [
                    new SheetSpec
                    {
                        Name = sourceSheet.Name,
                        Features = sourceSheet.Features.ToList(),
                        Columns = exportColumns
                    }
                ]
            }
        };
    }

    public static List<List<string>> ToExportValues(
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar)
    {
        var exportRows = new List<List<string>>(rows.Count);
        foreach (var row in rows)
        {
            var values = row.Values.ToList();
            values.Add(DateCalendarService.FormatDateTime(row.CreatedAt.ToLocalTime(), calendar));
            exportRows.Add(values);
        }

        return exportRows;
    }
}
