using System.Globalization;
using ExcelCreator.Localization;
using ExcelCreator.Models;
using ExcelCreator.Services;

namespace ExcelCreator.Validation;

public sealed class ValidationResult
{
    public bool IsValid { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ValidationResult Ok() => new() { IsValid = true };

    public static ValidationResult Fail(string message) => new() { IsValid = false, Message = message };
}

public static class TableValidator
{
    public static ValidationResult ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ValidationResult.Fail(PersianStrings.TableNameRequired);

        return ValidationResult.Ok();
    }

    public static ValidationResult ValidateTemplateCompatibility(SavedTable table, TemplateDefinition template)
    {
        if (!table.TemplateId.Equals(template.Id, StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Fail(PersianStrings.TableTemplateMismatch);

        if (table.TemplateVersion > 0 && table.TemplateVersion != template.Version)
            return ValidationResult.Fail(string.Format(PersianStrings.TableVersionMismatch, table.TemplateVersion, template.Version));

        var sheet = template.RequirePrimarySheet();
        if (table.ColumnHeaders.Count > 0 &&
            !table.ColumnHeaders.SequenceEqual(sheet.Columns.Select(c => c.Header)))
        {
            return ValidationResult.Fail(PersianStrings.TableColumnMismatch);
        }

        return ValidationResult.Ok();
    }

    public static ValidationResult ValidateRows(
        SheetSpec sheet,
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar = DateCalendarKind.Jalali) =>
        ValidateRows(sheet, rows.Select(r => r.Values).ToList(), calendar);

    public static ValidationResult ValidateRows(
        SheetSpec sheet,
        IReadOnlyList<List<string>> rows,
        DateCalendarKind calendar = DateCalendarKind.Jalali)
    {
        if (sheet.Columns.Count == 0)
            return ValidationResult.Fail(PersianStrings.TemplateHasNoColumns);

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (row.Count > sheet.Columns.Count)
                return ValidationResult.Fail(string.Format(PersianStrings.RowTooManyColumns, i + 1));

            if (row.All(string.IsNullOrWhiteSpace))
                return ValidationResult.Fail(string.Format(PersianStrings.RowEmpty, i + 1));

            var cellResult = ValidateRowCellValues(sheet, row, calendar, i + 1);
            if (!cellResult.IsValid)
                return cellResult;
        }

        return ValidationResult.Ok();
    }

    private static ValidationResult ValidateRowCellValues(
        SheetSpec sheet,
        IReadOnlyList<string> row,
        DateCalendarKind calendar,
        int rowNumber)
    {
        for (var col = 0; col < sheet.Columns.Count; col++)
        {
            var value = col < row.Count ? row[col].Trim() : string.Empty;
            if (string.IsNullOrWhiteSpace(value))
                continue;

            var column = sheet.Columns[col];
            if (ColumnTypes.IsDate(column.Type) &&
                !DateCalendarService.TryParseToGregorian(value, out _))
            {
                return ValidationResult.Fail(
                    string.Format(PersianStrings.InvalidDateValue, rowNumber, column.Header));
            }

            if (ColumnTypes.IsNumber(column.Type) &&
                !double.TryParse(DateCalendarService.NormalizeDigits(value), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
            {
                return ValidationResult.Fail(
                    string.Format(PersianStrings.InvalidNumberValue, rowNumber, column.Header));
            }

            if (ColumnTypes.IsCurrency(column.Type) &&
                !decimal.TryParse(DateCalendarService.NormalizeDigits(value), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
            {
                return ValidationResult.Fail(
                    string.Format(PersianStrings.InvalidCurrencyValue, rowNumber, column.Header));
            }
        }

        return ValidationResult.Ok();
    }

    public static List<TableRow> NormalizeRows(SheetSpec sheet, IReadOnlyList<TableRow> rows)
    {
        var normalized = new List<TableRow>(rows.Count);
        foreach (var row in rows)
        {
            normalized.Add(new TableRow
            {
                Values = NormalizeRowValues(sheet, row.Values),
                CreatedAt = row.CreatedAt
            });
        }

        return normalized;
    }

    public static List<List<string>> NormalizeRows(SheetSpec sheet, IReadOnlyList<List<string>> rows)
    {
        var normalized = new List<List<string>>(rows.Count);
        foreach (var row in rows)
            normalized.Add(NormalizeRowValues(sheet, row));
        return normalized;
    }

    private static List<string> NormalizeRowValues(SheetSpec sheet, IReadOnlyList<string> row)
    {
        var values = new List<string>(sheet.Columns.Count);
        for (var i = 0; i < sheet.Columns.Count; i++)
            values.Add(i < row.Count ? row[i].Trim() : string.Empty);
        return values;
    }
}
