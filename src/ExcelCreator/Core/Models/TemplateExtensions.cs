namespace ExcelCreator.Core.Models;

public static class TemplateExtensions
{
    public static SheetSpec GetPrimarySheet(this TemplateDefinition template) =>
        template.Workbook.Sheets[0];

    public static SheetSpec RequirePrimarySheet(this TemplateDefinition template)
    {
        if (template.Workbook.Sheets.Count == 0)
            throw new InvalidOperationException("الگو فاقد برگه داده است.");

        return template.Workbook.Sheets[0];
    }
}
