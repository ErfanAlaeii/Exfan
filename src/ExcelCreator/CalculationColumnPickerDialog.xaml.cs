using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Localization;
using ExcelCreator.Models;

namespace ExcelCreator;

public partial class CalculationColumnPickerDialog : Window
{
    private readonly CalculationInputKind _inputKind;

    public CalculationParameters? Parameters { get; private set; }

    public bool IsValid => Parameters is not null;

    public CalculationColumnPickerDialog(Window owner, SheetSpec sheet, CalculationInputKind inputKind)
    {
        InitializeComponent();
        Owner = owner;
        _inputKind = inputKind;
        Title = PersianStrings.CalculationColumnPickerTitle;
        PromptText.Text = PersianStrings.CalculationColumnPickerPrompt;
        PrimaryColumnLabel.Text = PersianStrings.CalculationPrimaryColumnLabel;
        SecondaryColumnLabel.Text = PersianStrings.CalculationSecondaryColumnLabel;
        ConfirmButton.Content = PersianStrings.Confirm;
        CancelButton.Content = PersianStrings.Cancel;

        var columns = GetEligibleColumnOptions(sheet, inputKind);

        PrimaryColumnBox.ItemsSource = columns;
        PrimaryColumnBox.DisplayMemberPath = nameof(ColumnOption.Header);
        SecondaryColumnBox.ItemsSource = columns;
        SecondaryColumnBox.DisplayMemberPath = nameof(ColumnOption.Header);

        if (columns.Count > 0)
            PrimaryColumnBox.SelectedIndex = 0;

        SecondaryColumnPanel.Visibility = RequiresSecondColumn(inputKind)
            ? Visibility.Visible
            : Visibility.Collapsed;

        if (RequiresSecondColumn(inputKind) && columns.Count > 1)
            SecondaryColumnBox.SelectedIndex = 1;

        UpdateConfirmButton();
    }

    private static bool RequiresSecondColumn(CalculationInputKind inputKind) =>
        inputKind is CalculationInputKind.TwoNumericColumns or CalculationInputKind.TwoTextColumns;

    private static List<ColumnOption> GetEligibleColumnOptions(SheetSpec sheet, CalculationInputKind inputKind) =>
        sheet.Columns
            .Select((column, index) => (column, index))
            .Where(pair => IsEligibleColumn(pair.column, inputKind))
            .Select(pair => new ColumnOption(pair.index, pair.column.Header))
            .ToList();

    private static bool IsEligibleColumn(ColumnSpec column, CalculationInputKind inputKind) =>
        inputKind switch
        {
            CalculationInputKind.SingleNumericColumn or CalculationInputKind.TwoNumericColumns =>
                Services.CalculationValueParser.IsCalculableColumn(column),
            CalculationInputKind.TwoTextColumns =>
                column.Type.Equals(ColumnTypes.Text, StringComparison.OrdinalIgnoreCase),
            _ => true
        };

    private void ColumnBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateConfirmButton();

    private void UpdateConfirmButton()
    {
        if (PrimaryColumnBox.SelectedItem is not ColumnOption primary)
        {
            ConfirmButton.IsEnabled = false;
            return;
        }

        if (!RequiresSecondColumn(_inputKind))
        {
            ConfirmButton.IsEnabled = true;
            return;
        }

        if (SecondaryColumnBox.SelectedItem is not ColumnOption secondary)
        {
            ConfirmButton.IsEnabled = false;
            return;
        }

        ConfirmButton.IsEnabled = primary.Index != secondary.Index;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        if (PrimaryColumnBox.SelectedItem is not ColumnOption primary)
            return;

        if (RequiresSecondColumn(_inputKind))
        {
            if (SecondaryColumnBox.SelectedItem is not ColumnOption secondary || primary.Index == secondary.Index)
                return;

            Parameters = new CalculationParameters
            {
                PrimaryColumnIndex = primary.Index,
                SecondaryColumnIndex = secondary.Index
            };
        }
        else
        {
            Parameters = new CalculationParameters
            {
                PrimaryColumnIndex = primary.Index,
                SecondaryColumnIndex = primary.Index
            };
        }

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private sealed class ColumnOption(int index, string header)
    {
        public int Index { get; } = index;
        public string Header { get; } = header;
    }
}
