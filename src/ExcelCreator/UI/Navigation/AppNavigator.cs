using System.Windows;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.UI.Windows;

namespace ExcelCreator.UI.Navigation;

public sealed class AppNavigator : IAppNavigator
{
    private readonly ISavedTableRepository _tables;
    private readonly IExcelExportFacade _export;
    private readonly ITablePrintFacade _print;
    private readonly IFileExportDialogService _dialogs;
    private readonly IAppLogger _logger;

    public AppNavigator(
        ISavedTableRepository tables,
        IExcelExportFacade export,
        ITablePrintFacade print,
        IFileExportDialogService dialogs,
        IAppLogger logger)
    {
        _tables = tables;
        _export = export;
        _print = print;
        _dialogs = dialogs;
        _logger = logger;
    }

    public void ShowTemplateAction(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar)
    {
        var window = new TemplateActionWindow(template, defaultCalendar, this);
        window.Owner = owner;
        window.ShowDialog();
    }

    public void ShowSavedTables(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar)
    {
        var window = new SavedTablesWindow(template, defaultCalendar, _tables, this, _dialogs, _logger);
        window.Owner = owner;
        window.ShowDialog();
    }

    public void ShowCreateTable(
        Window owner,
        TemplateDefinition template,
        DateCalendarKind defaultCalendar)
    {
        var window = new CreateTableWindow(template, defaultCalendar, _tables, _export, _print, _dialogs, this);
        window.Owner = owner;
        window.ShowDialog();
    }

    public void ShowTableEditor(Window owner, TemplateDefinition template, SavedTable table)
    {
        var window = new TableEditorWindow(template, table, _tables, _export, _print, _dialogs);
        window.Owner = owner;
        window.ShowDialog();
    }

    public void ShowPersonnelManagement(Window owner)
    {
        var window = new PersonnelWindow();
        window.Owner = owner;
        window.ShowDialog();
    }
}
