using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Localization;
using ExcelCreator.Core.Models;

namespace ExcelCreator.UI.Dialogs;

public partial class AddRowDialog : Window
{
    private readonly IReadOnlyList<ColumnSpec> _columns;
    private readonly DateCalendarKind _calendar;
    private readonly List<Control> _inputs = [];

    public List<string>? ResultRow { get; private set; }

    public AddRowDialog(
        Window owner,
        IReadOnlyList<ColumnSpec> columns,
        DateCalendarKind calendar,
        IReadOnlyList<string>? initialValues = null,
        bool isEdit = false)
    {
        InitializeComponent();
        Title = isEdit ? PersianStrings.EditRowDialogTitle : PersianStrings.AddRowDialogTitle;
        FormPromptText.Text = PersianStrings.AddRowFormPrompt;
        ConfirmButton.Content = PersianStrings.Confirm;
        CancelButton.Content = PersianStrings.Cancel;
        Owner = owner;
        _columns = columns;
        _calendar = calendar;
        BuildFields(initialValues);
    }

    private void BuildFields(IReadOnlyList<string>? initialValues)
    {
        for (var index = 0; index < _columns.Count; index++)
        {
            var column = _columns[index];
            var block = new StackPanel { Margin = new Thickness(0, 0, 0, 14) };

            block.Children.Add(new TextBlock
            {
                Text = column.Header,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 6),
                TextAlignment = TextAlignment.Right
            });

            var initial = initialValues is not null && index < initialValues.Count
                ? initialValues[index]
                : string.Empty;

            Control input;
            if (column.DropdownValues is { Count: > 0 })
            {
                var combo = new ComboBox
                {
                    FontSize = 14,
                    Padding = new Thickness(8, 6, 8, 6),
                    IsEditable = false
                };
                combo.Items.Add(string.Empty);
                foreach (var value in column.DropdownValues)
                    combo.Items.Add(value);
                if (!string.IsNullOrEmpty(initial))
                    combo.SelectedItem = initial;
                input = combo;
            }
            else
            {
                var box = new TextBox
                {
                    FontSize = 14,
                    Padding = new Thickness(10, 8, 10, 8),
                    FlowDirection = FlowDirection.RightToLeft,
                    TextAlignment = TextAlignment.Right,
                    Text = initial
                };
                input = box;

        if (column.Type.Equals(ColumnTypes.Date, StringComparison.OrdinalIgnoreCase))
                {
                    block.Children.Add(new TextBlock
                    {
                        Text = _calendar == DateCalendarKind.Gregorian
                            ? PersianStrings.DateFormatHintGregorian
                            : PersianStrings.DateFormatHintJalali,
                        FontSize = 11,
                        Foreground = (System.Windows.Media.Brush)FindResource("MutedBrush"),
                        Margin = new Thickness(0, 0, 0, 4),
                        TextAlignment = TextAlignment.Right
                    });
                }
            }

            block.Children.Add(input);
            _inputs.Add(input);
            FieldsPanel.Children.Add(block);
        }
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        var values = new List<string>();
        for (var i = 0; i < _columns.Count; i++)
            values.Add(ReadValue(_inputs[i]));

        if (values.All(string.IsNullOrWhiteSpace))
        {
            MessageBox.Show(
                PersianStrings.AddRowValidation,
                PersianStrings.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        ResultRow = values;
        DialogResult = true;
        Close();
    }

    private static string ReadValue(Control control) => control switch
    {
        TextBox box => box.Text.Trim(),
        ComboBox combo => combo.SelectedItem?.ToString()?.Trim() ?? string.Empty,
        _ => string.Empty
    };

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
