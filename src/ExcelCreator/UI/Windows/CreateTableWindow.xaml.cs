using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Application.Calculations;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Composition;
using ExcelCreator.Localization;
using ExcelCreator.Core.Models;
using ExcelCreator.Core.Validation;

namespace ExcelCreator.UI.Windows;

public partial class CreateTableWindow : Window
{
    private readonly TemplateDefinition _template;
    private readonly SheetSpec _sheet;
    private readonly ISavedTableRepository _tables;
    private readonly IExcelExportFacade _export;
    private readonly IFileExportDialogService _dialogs;
    private readonly IAppNavigator _navigator;
    private int _step;

    public CreateTableWindow(
        TemplateDefinition template,
        DateCalendarKind defaultCalendar,
        ISavedTableRepository tables,
        IExcelExportFacade export,
        IFileExportDialogService dialogs,
        IAppNavigator navigator)
    {
        InitializeComponent();
        _template = template;
        _sheet = template.RequirePrimarySheet();
        _tables = tables;
        _export = export;
        _dialogs = dialogs;
        _navigator = navigator;

        Title = PersianStrings.CreateTableWindowTitle;
        TableNameLabel.Text = PersianStrings.CreateTableStepNameTitle;
        TableNameHint.Text = PersianStrings.CreateTableNameHint;
        CalendarSelector.SetCalendar(defaultCalendar);
        CancelButton.Content = PersianStrings.Cancel;
        BackButton.Content = PersianStrings.Back;
        ExportButton.Content = PersianStrings.ExportExcel;
        UpdateStepUi();
    }

    private void CalendarSelector_CalendarChanged(object sender, EventArgs e)
    {
        if (_step == 2)
            RowsEditor.SetCalendar(CalendarSelector.SelectedCalendar);
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
                Calendar = CalendarSelector.SelectedCalendar
            });
    }

    private void RowsEditor_RowsChanged(object sender, EventArgs e) =>
        CalculationActions.RefreshActions();

    private void UpdateStepUi()
    {
        StepName.Visibility = Visibility.Collapsed;
        CalendarSelector.Visibility = Visibility.Collapsed;
        StepDataEntry.Visibility = Visibility.Collapsed;
        BackButton.Visibility = _step > 0 ? Visibility.Visible : Visibility.Collapsed;
        ExportButton.Visibility = _step == 2 ? Visibility.Visible : Visibility.Collapsed;

        switch (_step)
        {
            case 0:
                StepName.Visibility = Visibility.Visible;
                StepTitle.Text = PersianStrings.CreateTableStepNameTitle;
                StepSubtitle.Text = PersianStrings.CreateTableStepNameSubtitle;
                StepIndicator.Text = PersianStrings.CreateTableStep1Indicator;
                NextButton.Content = PersianStrings.Next;
                NextButton.IsEnabled = !string.IsNullOrWhiteSpace(TableNameBox.Text);
                break;
            case 1:
                CalendarSelector.Visibility = Visibility.Visible;
                StepTitle.Text = PersianStrings.CreateTableStepCalendarTitle;
                StepSubtitle.Text = PersianStrings.CreateTableStepCalendarSubtitle;
                StepIndicator.Text = PersianStrings.CreateTableStep2Indicator;
                NextButton.Content = PersianStrings.Next;
                NextButton.IsEnabled = true;
                break;
            case 2:
                StepDataEntry.Visibility = Visibility.Visible;
                RowsEditor.Load(_sheet, CalendarSelector.SelectedCalendar);
                InitializeCalculationActions();
                StepTitle.Text = PersianStrings.Step3Title;
                StepSubtitle.Text = PersianStrings.Step3Subtitle;
                StepIndicator.Text = PersianStrings.CreateTableStep3Indicator;
                NextButton.Content = PersianStrings.CreateTableButton;
                NextButton.IsEnabled = true;
                break;
        }
    }

    private void TableNameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_step == 0)
            NextButton.IsEnabled = !string.IsNullOrWhiteSpace(TableNameBox.Text);
    }

    private bool ValidateTableName()
    {
        var validation = TableValidator.ValidateName(TableNameBox.Text);
        if (!validation.IsValid)
        {
            _dialogs.NotifyWarning(validation.Message);
            return false;
        }

        if (_tables.NameExistsForTemplate(_template.Id, TableNameBox.Text))
        {
            _dialogs.NotifyWarning(PersianStrings.TableNameExists);
            return false;
        }

        return true;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (_step > 0)
        {
            _step--;
            UpdateStepUi();
        }
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        if (_step == 2)
        {
            CreateTable();
            return;
        }

        if (_step == 0 && !ValidateTableName())
            return;

        _step++;
        UpdateStepUi();
    }

    private void CreateTable()
    {
        if (!ValidateTableName())
            return;

        try
        {
            var table = new SavedTable
            {
                TemplateId = _template.Id,
                TemplateVersion = _template.Version,
                Name = TableNameBox.Text.Trim(),
                DateCalendar = CalendarSelector.SelectedCalendar,
                Rows = RowsEditor.Rows
                    .Select(r => TableRow.FromValues(r.Values, r.CreatedAt))
                    .ToList()
            };

            _tables.Save(table, _template);
            _dialogs.NotifyInfo(PersianStrings.CreateTableSuccess);

            Close();
            _navigator.ShowTableEditor(Owner, _template, table);
        }
        catch (Exception ex)
        {
            _dialogs.NotifyWarning(ex.Message);
        }
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateTableName())
            return;

        _export.ExportWithDialog(
            this,
            _template,
            RowsEditor.Rows,
            CalendarSelector.SelectedCalendar,
            TableNameBox.Text.Trim());
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
