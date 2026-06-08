using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Tables;

public static class ColumnDropdownResolver
{
    public static TemplateDefinition ResolveTemplate(TemplateDefinition template, IPersonnelRepository personnel)
    {
        var sourceSheet = template.RequirePrimarySheet();
        var resolvedColumns = ResolveColumns(sourceSheet.Columns, personnel);

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
                        Columns = resolvedColumns.ToList()
                    }
                ]
            }
        };
    }

    public static IReadOnlyList<ColumnSpec> ResolveColumns(
        IReadOnlyList<ColumnSpec> columns,
        IPersonnelRepository personnel) =>
        columns.Select(column => ResolveColumn(column, personnel)).ToList();

    public static bool RequiresPersonnel(IReadOnlyList<ColumnSpec> columns) =>
        columns.Any(column => ColumnDropdownSources.IsPersonnel(column.DropdownSource));

    private static ColumnSpec ResolveColumn(ColumnSpec column, IPersonnelRepository personnel)
    {
        if (column.DropdownValues is { Count: > 0 } || !ColumnDropdownSources.IsPersonnel(column.DropdownSource))
            return column;

        return CloneColumn(column, personnel.GetNames());
    }

    private static ColumnSpec CloneColumn(ColumnSpec column, IReadOnlyList<string> dropdownValues) =>
        new()
        {
            Header = column.Header,
            Type = column.Type,
            Width = column.Width,
            Formula = column.Formula,
            DropdownSource = column.DropdownSource,
            DropdownValues = dropdownValues.ToList(),
            Multiline = column.Multiline
        };
}
