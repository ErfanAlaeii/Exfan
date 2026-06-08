using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Tables;

public static class TableSchemaResolver
{
    public static SheetSpec ResolveSheet(TemplateDefinition template, SavedTable? table = null)
    {
        var templateSheet = template.RequirePrimarySheet();
        if (table?.CustomColumns is { Count: > 0 })
        {
            return new SheetSpec
            {
                Name = templateSheet.Name,
                Features = templateSheet.Features.ToList(),
                Columns = CloneColumns(table.CustomColumns)
            };
        }

        return CloneSheet(templateSheet);
    }

    public static SheetSpec CreateEditableSheet(TemplateDefinition template, SavedTable? table = null) =>
        ResolveSheet(template, table);

    public static TemplateDefinition WithPrimarySheet(TemplateDefinition template, SheetSpec sheet) =>
        new()
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
                        Name = sheet.Name,
                        Features = sheet.Features.ToList(),
                        Columns = CloneColumns(sheet.Columns)
                    }
                ]
            }
        };

    public static List<ColumnSpec> CloneColumns(IEnumerable<ColumnSpec> columns) =>
        columns.Select(CloneColumn).ToList();

    public static SheetSpec CloneSheet(SheetSpec sheet) =>
        new()
        {
            Name = sheet.Name,
            Features = sheet.Features.ToList(),
            Columns = CloneColumns(sheet.Columns)
        };

    private static ColumnSpec CloneColumn(ColumnSpec column) =>
        new()
        {
            Header = column.Header,
            Type = column.Type,
            Width = column.Width,
            Formula = column.Formula,
            DropdownSource = column.DropdownSource,
            DropdownValues = column.DropdownValues?.ToList(),
            Multiline = column.Multiline
        };
}
