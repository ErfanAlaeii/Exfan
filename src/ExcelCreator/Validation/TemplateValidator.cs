using ExcelCreator.Models;
using ExcelCreator.Validation;

namespace ExcelCreator.Validation;

public static class TemplateValidator
{
    public static ValidationResult Validate(TemplateDefinition template)
    {
        if (string.IsNullOrWhiteSpace(template.Id))
            return ValidationResult.Fail("الگو باید شناسه داشته باشد.");

        if (string.IsNullOrWhiteSpace(template.Title))
            return ValidationResult.Fail("الگو باید عنوان داشته باشد.");

        if (template.Workbook.Sheets.Count == 0)
            return ValidationResult.Fail("الگو باید حداقل یک برگه داشته باشد.");

        foreach (var sheet in template.Workbook.Sheets)
        {
            var sheetResult = ValidateSheet(sheet);
            if (!sheetResult.IsValid)
                return sheetResult;
        }

        return ValidationResult.Ok();
    }

    public static ValidationResult ValidateSheet(SheetSpec sheet)
    {
        if (string.IsNullOrWhiteSpace(sheet.Name))
            return ValidationResult.Fail("نام برگه الزامی است.");

        if (sheet.Columns.Count == 0)
            return ValidationResult.Fail($"برگه «{sheet.Name}» فاقد ستون است.");

        foreach (var column in sheet.Columns)
        {
            if (string.IsNullOrWhiteSpace(column.Header))
                return ValidationResult.Fail("سرستون‌ها نمی‌توانند خالی باشند.");

            if (!ColumnTypes.IsKnown(column.Type))
                return ValidationResult.Fail($"نوع ستون «{column.Type}» پشتیبانی نمی‌شود.");
        }

        foreach (var feature in sheet.Features)
        {
            if (!SheetFeatures.IsKnown(feature))
                return ValidationResult.Fail($"امکان برگه «{feature}» پشتیبانی نمی‌شود.");
        }

        return ValidationResult.Ok();
    }
}
