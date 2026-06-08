using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using ExcelCreator.Localization;
using ExcelCreator.UI.Controls;

namespace ExcelCreator.UI.Dialogs;

public partial class SearchRowsDialog : Window
{
    private readonly IReadOnlyList<TableRowSearchField> _fields;
    private readonly DateCalendarKind _calendar;
    private Control? _valueInput;

    public TableRowSearchCriteria? Result { get; private set; }

    public SearchRowsDialog(Window owner, IReadOnlyList<TableRowSearchField> fields, DateCalendarKind calendar)
    {
        InitializeComponent();
        Owner = owner;
        _fields = fields;
        _calendar = calendar;

        Title = PersianStrings.SearchRowsDialogTitle;
        PromptText.Text = PersianStrings.SearchRowsDialogPrompt;
        FieldLabel.Text = PersianStrings.SearchFieldLabel;
        ValueLabel.Text = PersianStrings.SearchValueLabel;
        ConfirmButton.Content = PersianStrings.SearchRows;
        CancelButton.Content = PersianStrings.Cancel;

        FieldBox.ItemsSource = fields
            .Select((field, index) => new FieldOption(index, field.Header))
            .ToList();
        FieldBox.DisplayMemberPath = nameof(FieldOption.Header);

        if (fields.Count > 0)
            FieldBox.SelectedIndex = 0;
    }

    private void FieldBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FieldBox.SelectedItem is not FieldOption option)
            return;

        _valueInput = CreateValueInput(_fields[option.Index]);
        ValueInputHost.Content = _valueInput;
    }

    private Control CreateValueInput(TableRowSearchField field)
    {
        if (field.InputKind == TableRowSearchInputKind.Dropdown && field.DropdownValues is not null)
        {
            var combo = new ComboBox
            {
                FontSize = 14,
                Padding = new Thickness(8, 6, 8, 6),
                FlowDirection = FlowDirection.RightToLeft
            };

            foreach (var value in field.DropdownValues)
                combo.Items.Add(value);

            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;

            return combo;
        }

        if (field.InputKind == TableRowSearchInputKind.Date)
        {
            var input = new CalendarDateInputControl();
            input.SetCalendar(_calendar);
            return input;
        }

        return new TextBox
        {
            FontSize = 14,
            Padding = new Thickness(10, 8, 10, 8),
            FlowDirection = FlowDirection.RightToLeft,
            TextAlignment = TextAlignment.Right
        };
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        if (FieldBox.SelectedItem is not FieldOption option || _valueInput is null)
            return;

        var field = _fields[option.Index];
        var searchValue = ReadSearchValue(field, _valueInput);
        if (string.IsNullOrWhiteSpace(searchValue))
        {
            MessageBox.Show(
                PersianStrings.SearchValidation,
                PersianStrings.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        Result = new TableRowSearchCriteria
        {
            Target = field.Target,
            ColumnIndex = field.ColumnIndex,
            SearchValue = searchValue
        };

        DialogResult = true;
        Close();
    }

    private static string ReadSearchValue(TableRowSearchField field, Control input)
    {
        if (field.InputKind == TableRowSearchInputKind.Date && input is CalendarDateInputControl dateInput)
            return dateInput.GetSearchValue();

        return input switch
        {
            TextBox box => box.Text.Trim(),
            ComboBox combo => combo.SelectedItem?.ToString()?.Trim() ?? string.Empty,
            _ => string.Empty
        };
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private sealed class FieldOption(int index, string header)
    {
        public int Index { get; } = index;

        public string Header { get; } = header;
    }
}
