using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Core.Validation;
using ExcelCreator.Localization;

namespace ExcelCreator.Application.Print;

public sealed class TablePrintFacade : ITablePrintFacade
{
    private readonly IPrintService _printService;
    private readonly IFileExportDialogService _dialogs;

    public TablePrintFacade(IPrintService printService, IFileExportDialogService dialogs)
    {
        _printService = printService;
        _dialogs = dialogs;
    }

    public bool PrintWithDialog(
        object ownerWindow,
        TemplateDefinition template,
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar,
        string title)
    {
        if (rows.Count == 0)
        {
            _dialogs.NotifyWarning(PersianStrings.PrintNoRows);
            return false;
        }

        var sheet = template.RequirePrimarySheet();
        var validation = TableValidator.ValidateRows(sheet, rows, calendar);
        if (!validation.IsValid)
        {
            _dialogs.NotifyValidationError(validation.Message);
            return false;
        }

        var normalized = TableValidator.NormalizeRows(sheet, rows);
        var model = TablePrintModelBuilder.Build(sheet, normalized, calendar, title.Trim());
        return _printService.Print(model, ownerWindow);
    }
}
