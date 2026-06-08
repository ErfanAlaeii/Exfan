using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using ExcelCreator.Localization;

namespace ExcelCreator.UI.Dialogs;

public partial class FieldSettingsDialog : Window
{
    private readonly IReadOnlyList<ColumnSpec> _columns;

    public TableColumnChangeAction? SelectedAction { get; private set; }

    public int? SelectedColumnIndex { get; private set; }

    public string? EnteredName { get; private set; }

    public FieldSettingsDialog(Window owner, IReadOnlyList<ColumnSpec> columns)
    {
        InitializeComponent();
        Owner = owner;
        _columns = columns;

        Title = PersianStrings.FieldSettingsDialogTitle;
        PromptText.Text = PersianStrings.FieldSettingsDialogPrompt;
        ActionLabel.Text = PersianStrings.FieldSettingsActionLabel;
        FieldSelectLabel.Text = PersianStrings.FieldSettingsSelectFieldLabel;
        NameInputLabel.Text = PersianStrings.FieldSettingsFieldNameLabel;
        ConfirmButton.Content = PersianStrings.Confirm;
        CancelButton.Content = PersianStrings.Cancel;

        ActionBox.ItemsSource = new[]
        {
            new ActionOption(TableColumnChangeAction.Add, PersianStrings.FieldSettingsAdd),
            new ActionOption(TableColumnChangeAction.Delete, PersianStrings.FieldSettingsDelete),
            new ActionOption(TableColumnChangeAction.Edit, PersianStrings.FieldSettingsEdit)
        };
        ActionBox.DisplayMemberPath = nameof(ActionOption.Label);
        ActionBox.SelectedIndex = 0;

        FieldBox.ItemsSource = columns
            .Select((column, index) => new FieldOption(index, column.Header))
            .ToList();
        FieldBox.DisplayMemberPath = nameof(FieldOption.Header);

        if (columns.Count > 0)
            FieldBox.SelectedIndex = 0;

        UpdateDetailsPanel();
    }

    private void ActionBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        UpdateDetailsPanel();

    private void FieldBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ActionBox.SelectedItem is not ActionOption { Action: TableColumnChangeAction.Edit })
            return;

        if (FieldBox.SelectedItem is FieldOption selected)
            NameBox.Text = selected.Header;
    }

    private void UpdateDetailsPanel()
    {
        if (ActionBox.SelectedItem is not ActionOption action)
            return;

        switch (action.Action)
        {
            case TableColumnChangeAction.Add:
                FieldSelectLabel.Visibility = Visibility.Collapsed;
                FieldBox.Visibility = Visibility.Collapsed;
                NameInputLabel.Text = PersianStrings.FieldSettingsFieldNameLabel;
                NameInputLabel.Visibility = Visibility.Visible;
                NameBox.Visibility = Visibility.Visible;
                NameBox.Text = string.Empty;
                break;
            case TableColumnChangeAction.Delete:
                FieldSelectLabel.Visibility = Visibility.Visible;
                FieldBox.Visibility = Visibility.Visible;
                NameInputLabel.Visibility = Visibility.Collapsed;
                NameBox.Visibility = Visibility.Collapsed;
                break;
            case TableColumnChangeAction.Edit:
                FieldSelectLabel.Visibility = Visibility.Visible;
                FieldBox.Visibility = Visibility.Visible;
                NameInputLabel.Text = PersianStrings.FieldSettingsNewNameLabel;
                NameInputLabel.Visibility = Visibility.Visible;
                NameBox.Visibility = Visibility.Visible;
                if (FieldBox.SelectedItem is FieldOption selected)
                    NameBox.Text = selected.Header;
                else
                    NameBox.Text = string.Empty;
                break;
        }
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        if (ActionBox.SelectedItem is not ActionOption action)
            return;

        SelectedAction = action.Action;

        switch (action.Action)
        {
            case TableColumnChangeAction.Add:
                EnteredName = NameBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(EnteredName))
                {
                    ShowValidation(PersianStrings.FieldSettingsNameRequired);
                    return;
                }

                break;

            case TableColumnChangeAction.Delete:
                if (FieldBox.SelectedItem is not FieldOption deleteOption)
                {
                    ShowValidation(PersianStrings.FieldSettingsSelectFieldPrompt);
                    return;
                }

                SelectedColumnIndex = deleteOption.Index;
                break;

            case TableColumnChangeAction.Edit:
                if (FieldBox.SelectedItem is not FieldOption editOption)
                {
                    ShowValidation(PersianStrings.FieldSettingsSelectFieldPrompt);
                    return;
                }

                EnteredName = NameBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(EnteredName))
                {
                    ShowValidation(PersianStrings.FieldSettingsNameRequired);
                    return;
                }

                SelectedColumnIndex = editOption.Index;
                break;
        }

        DialogResult = true;
        Close();
    }

    private static void ShowValidation(string message) =>
        MessageBox.Show(message, PersianStrings.AppName, MessageBoxButton.OK, MessageBoxImage.Information);

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private sealed class ActionOption(TableColumnChangeAction action, string label)
    {
        public TableColumnChangeAction Action { get; } = action;

        public string Label { get; } = label;
    }

    private sealed class FieldOption(int index, string header)
    {
        public int Index { get; } = index;

        public string Header { get; } = header;
    }
}
