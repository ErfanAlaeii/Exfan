using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface ITablePrintFacade
{
    bool PrintWithDialog(
        object ownerWindow,
        TemplateDefinition template,
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar,
        string title);
}
