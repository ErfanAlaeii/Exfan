using System.Windows;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface IExcelExportFacade
{
    bool ExportWithDialog(
        Window owner,
        TemplateDefinition template,
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar,
        string suggestedFileName);
}
