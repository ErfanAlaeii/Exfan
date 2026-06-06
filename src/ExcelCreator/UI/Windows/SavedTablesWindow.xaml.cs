using System.Windows;
using System.Windows.Input;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Localization;
using ExcelCreator.Application.Common;
using ExcelCreator.Core.Models;

namespace ExcelCreator.UI.Windows;

public partial class SavedTablesWindow : Window
{
    private readonly TemplateDefinition _template;
    private readonly DateCalendarKind _defaultCalendar;
    private readonly ISavedTableRepository _tables;
    private readonly IAppNavigator _navigator;
    private readonly IFileExportDialogService _dialogs;
    private readonly IAppLogger _logger;
    private List<SavedTableListItem> _items = [];

    public SavedTablesWindow(
        TemplateDefinition template,
        DateCalendarKind defaultCalendar,
        ISavedTableRepository tables,
        IAppNavigator navigator,
        IFileExportDialogService dialogs,
        IAppLogger logger)
    {
        InitializeComponent();
        _template = template;
        _defaultCalendar = defaultCalendar;
        _tables = tables;
        _navigator = navigator;
        _dialogs = dialogs;
        _logger = logger;

        Title = template.Title;
        HeaderText.Text = string.Format(PersianStrings.SavedTablesHeader, template.Title);
        ListHintText.Text = PersianStrings.SavedTablesHint;
        EmptyText.Text = PersianStrings.NoSavedTables;
        BackButton.Content = PersianStrings.Back;
        DeleteButton.Content = PersianStrings.DeleteTable;
        OpenButton.Content = PersianStrings.OpenTable;
        LoadTables();
    }

    private void LoadTables()
    {
        try
        {
            _items = _tables.GetByTemplate(_template.Id)
                .Select(t => new SavedTableListItem
                {
                    Table = t,
                    Name = t.Name,
                    Subtitle = $"{PersianStrings.RowCountLabel}: {t.Rows.Count} — {FormatTableTimestamp(t)}"
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to load saved tables", ex);
            _dialogs.NotifyError(ex.Message);
            _items = [];
        }

        TablesList.ItemsSource = _items;
        EmptyText.Visibility = _items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        TablesList.Visibility = _items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private static string FormatTableTimestamp(SavedTable table) =>
        DateCalendarService.FormatDateTime(table.UpdatedAt.ToLocalTime(), table.DateCalendar);

    private void OpenSelected()
    {
        if (TablesList.SelectedItem is not SavedTableListItem item)
        {
            _dialogs.NotifyInfo(PersianStrings.SelectTablePrompt);
            return;
        }

        _navigator.ShowTableEditor(Owner, _template, item.Table);
        LoadTables();
    }

    private void Open_Click(object sender, RoutedEventArgs e) => OpenSelected();

    private void TablesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenSelected();

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (TablesList.SelectedItem is not SavedTableListItem item)
        {
            _dialogs.NotifyInfo(PersianStrings.SelectTablePrompt);
            return;
        }

        if (_dialogs.Confirm(
                string.Format(PersianStrings.DeleteTableConfirm, item.Name),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        _tables.Delete(item.Table.Id);
        LoadTables();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        Close();
        _navigator.ShowTemplateAction(Owner, _template, _defaultCalendar);
    }

    private sealed class SavedTableListItem
    {
        public required SavedTable Table { get; init; }
        public required string Name { get; init; }
        public required string Subtitle { get; init; }
    }
}
