using System.Windows;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Composition;
using ExcelCreator.Localization;
using ExcelCreator.Application.Calculations;
using ExcelCreator.Application.Common;
using ExcelCreator.Core.Models;

namespace ExcelCreator.UI.Windows;

public partial class TableEditorWindow : Window
{
    private readonly TemplateDefinition _template;
    private readonly SavedTable _table;
    private readonly SheetSpec _sheet;
    private readonly ISavedTableRepository _tables;
    private readonly IExcelExportFacade _export;
    private readonly ITablePrintFacade _print;
    private readonly IFileExportDialogService _dialogs;
    private bool _dirty;
    private bool _columnsCustomized;

    public TableEditorWindow(
        TemplateDefinition template,
        SavedTable table,
        ISavedTableRepository tables,
        IExcelExportFacade export,
        ITablePrintFacade print,
        IFileExportDialogService dialogs)
    {
        InitializeComponent();
        _template = template;
        _table = table;
        _sheet = TableSchemaResolver.CreateEditableSheet(template, table);
        _tables = tables;
        _export = export;
        _print = print;
        _dialogs = dialogs;

        Title = table.Name;
        TableNameText.Text = table.Name;
        CalendarHintText.Text = $"{PersianStrings.CalendarSectionTitle}: {DateCalendarService.GetCalendarLabel(table.DateCalendar)}";
        BackButton.Content = PersianStrings.BackToList;
        SaveButton.Content = PersianStrings.SaveChanges;
        ExportButton.Content = PersianStrings.ExportExcel;
        PrintButton.Content = PersianStrings.PrintTable;

        RowsEditor.Load(_sheet, table.DateCalendar, table.Rows);
        RowsEditor.ColumnsChanged += RowsEditor_ColumnsChanged;
        InitializeCalculationActions();
    }

    private void RowsEditor_ColumnsChanged(object? sender, EventArgs e)
    {
        _columnsCustomized = true;
        _dirty = true;
        InitializeCalculationActions();
    }

    private TemplateDefinition ExportTemplate =>
        TableSchemaResolver.WithPrimarySheet(_template, _sheet);

    private void InitializeCalculationActions()
    {
        CalculationActions.Initialize(
            ServiceRegistration.GetRequiredService<ICalculationActionRegistry>(),
            ServiceRegistration.GetRequiredService<ICalculationEngine>(),
            () => new CalculationContext
            {
                Sheet = _sheet,
                Rows = RowsEditor.Rows,
                Calendar = _table.DateCalendar
            });
    }

    private void RowsEditor_RowsChanged(object sender, EventArgs e)
    {
        _dirty = true;
        CalculationActions.RefreshActions();
    }

    private void ApplyTableState()
    {
        _table.Rows = RowsEditor.Rows
            .Select(r => TableRow.FromValues(r.Values, r.CreatedAt))
            .ToList();

        if (_columnsCustomized || _table.CustomColumns is { Count: > 0 })
            _table.CustomColumns = TableSchemaResolver.CloneColumns(_sheet.Columns);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ApplyTableState();
            _tables.Save(_table, _template);
            _dirty = false;
            _dialogs.NotifyInfo(PersianStrings.SaveChangesSuccess);
        }
        catch (Exception ex)
        {
            _dialogs.NotifyWarning(ex.Message);
        }
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (_dirty)
        {
            try
            {
                ApplyTableState();
                _tables.Save(_table, _template);
                _dirty = false;
            }
            catch (Exception ex)
            {
                _dialogs.NotifyWarning(ex.Message);
                return;
            }
        }

        _export.ExportWithDialog(this, ExportTemplate, RowsEditor.Rows, _table.DateCalendar, _table.Name);
    }

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        _print.PrintWithDialog(this, ExportTemplate, RowsEditor.Rows, _table.DateCalendar, _table.Name);
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (_dirty)
        {
            var result = _dialogs.Confirm(
                PersianStrings.UnsavedChangesPrompt,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
                return;

            if (result == MessageBoxResult.Yes)
                Save_Click(sender, e);
        }

        Close();
    }
}
