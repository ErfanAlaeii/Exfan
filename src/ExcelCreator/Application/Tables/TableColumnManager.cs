using ExcelCreator.Core.Models;
using ExcelCreator.Core.Validation;
using ExcelCreator.Localization;

namespace ExcelCreator.Application.Tables;

public enum TableColumnChangeAction
{
    Add,
    Delete,
    Edit
}

public static class TableColumnManager
{
    public const int MinimumColumnCount = 1;

    public static ValidationResult AddColumn(SheetSpec sheet, string header)
    {
        var normalized = NormalizeHeader(header);
        var nameValidation = ValidateHeader(normalized);
        if (!nameValidation.IsValid)
            return nameValidation;

        if (HeaderExists(sheet, normalized))
            return ValidationResult.Fail(PersianStrings.FieldSettingsDuplicateName);

        sheet.Columns.Add(new ColumnSpec
        {
            Header = normalized,
            Type = ColumnTypes.Text,
            Width = 18
        });

        return ValidationResult.Ok();
    }

    public static ValidationResult RemoveColumn(SheetSpec sheet, int columnIndex, IList<TableRow> rows)
    {
        if (columnIndex < 0 || columnIndex >= sheet.Columns.Count)
            return ValidationResult.Fail(PersianStrings.FieldSettingsColumnNotFound);

        if (sheet.Columns.Count <= MinimumColumnCount)
            return ValidationResult.Fail(PersianStrings.FieldSettingsMinimumColumns);

        sheet.Columns.RemoveAt(columnIndex);
        AdjustRowsAfterRemove(rows, columnIndex);
        return ValidationResult.Ok();
    }

    public static ValidationResult RenameColumn(SheetSpec sheet, int columnIndex, string newHeader)
    {
        if (columnIndex < 0 || columnIndex >= sheet.Columns.Count)
            return ValidationResult.Fail(PersianStrings.FieldSettingsColumnNotFound);

        var normalized = NormalizeHeader(newHeader);
        var nameValidation = ValidateHeader(normalized);
        if (!nameValidation.IsValid)
            return nameValidation;

        if (HeaderExists(sheet, normalized, columnIndex))
            return ValidationResult.Fail(PersianStrings.FieldSettingsDuplicateName);

        sheet.Columns[columnIndex].Header = normalized;
        return ValidationResult.Ok();
    }

    public static void NormalizeAllRows(SheetSpec sheet, IList<TableRow> rows)
    {
        for (var index = 0; index < rows.Count; index++)
            rows[index] = TableRow.FromValues(
                TableValidator.NormalizeRows(sheet, [rows[index].Values])[0],
                rows[index].CreatedAt);
    }

    private static ValidationResult ValidateHeader(string header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return ValidationResult.Fail(PersianStrings.FieldSettingsNameRequired);

        return ValidationResult.Ok();
    }

    private static string NormalizeHeader(string header) => header.Trim();

    private static bool HeaderExists(SheetSpec sheet, string header, int? excludeIndex = null) =>
        sheet.Columns
            .Select((column, index) => (column, index))
            .Any(pair =>
                (excludeIndex is null || pair.index != excludeIndex) &&
                pair.column.Header.Equals(header, StringComparison.CurrentCultureIgnoreCase));

    private static void AdjustRowsAfterRemove(IList<TableRow> rows, int removedIndex)
    {
        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var values = rows[rowIndex].Values.ToList();
            if (removedIndex < values.Count)
                values.RemoveAt(removedIndex);

            rows[rowIndex] = TableRow.FromValues(values, rows[rowIndex].CreatedAt);
        }
    }
}
