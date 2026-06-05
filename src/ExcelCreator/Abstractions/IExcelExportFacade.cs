using System.Windows;
using ExcelCreator.Models;

namespace ExcelCreator.Abstractions;

public interface IExcelExportFacade
{
    bool ExportWithDialog(
        Window owner,
        TemplateDefinition template,
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar,
        string suggestedFileName);
}
