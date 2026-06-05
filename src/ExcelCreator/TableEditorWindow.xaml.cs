using System.Windows;
using ExcelCreator.Abstractions;
using ExcelCreator.Composition;
using ExcelCreator.Localization;
using ExcelCreator.Models;
using ExcelCreator.Services;

namespace ExcelCreator;

public partial class TableEditorWindow : Window
{
    private readonly TemplateDefinition _template;
    private readonly SavedTable _table;
    private readonly SheetSpec _sheet;
    private readonly ISavedTableRepository _tables;
    private readonly IExcelExportFacade _export;
    private readonly IFileExportDialogService _dialogs;
    private bool _dirty;

    public TableEditorWindow(
        TemplateDefinition template,
        SavedTable table,
        ISavedTableRepository tables,
        IExcelExportFacade export,
        IFileExportDialogService dialogs)
    {
        InitializeComponent();
        _template = template;
        _table = table;
        _sheet = template.RequirePrimarySheet();
        _tables = tables;
        _export = export;
        _dialogs = dialogs;

        Title = table.Name;
        TableNameText.Text = table.Name;
        CalendarHintText.Text = $"{PersianStrings.CalendarSectionTitle}: {DateCalendarService.GetCalendarLabel(table.DateCalendar)}";
        BackButton.Content = PersianStrings.BackToList;
        SaveButton.Content = PersianStrings.SaveChanges;
        ExportButton.Content = PersianStrings.ExportExcel;

        RowsEditor.Load(_sheet, table.DateCalendar, table.Rows);
        InitializeCalculationActions();
    }

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

    private void ApplyRowsToTable() =>
        _table.Rows = RowsEditor.Rows
            .Select(r => TableRow.FromValues(r.Values, r.CreatedAt))
            .ToList();

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ApplyRowsToTable();
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
                ApplyRowsToTable();
                _tables.Save(_table, _template);
                _dirty = false;
            }
            catch (Exception ex)
            {
                _dialogs.NotifyWarning(ex.Message);
                return;
            }
        }

        _export.ExportWithDialog(this, _template, RowsEditor.Rows, _table.DateCalendar, _table.Name);
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
