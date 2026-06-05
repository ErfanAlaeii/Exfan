using System.Data;
using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Infrastructure;
using ExcelCreator.Localization;
using ExcelCreator.Models;
using ExcelCreator.Services;

namespace ExcelCreator.Controls;

public partial class TableRowsEditor : UserControl
{
    private readonly List<TableRow> _rows = [];
    private SheetSpec? _sheet;
    private DateCalendarKind _calendar = DateCalendarKind.Jalali;
    private DataTable? _viewTable;

    public TableRowsEditor()
    {
        InitializeComponent();
        AddRowButton.Content = PersianStrings.AddRow;
        EditRowButton.Content = PersianStrings.EditRow;
        DeleteRowButton.Content = PersianStrings.DeleteRow;
        EmptyText.Text = PersianStrings.DataEntryEmpty;
    }

    public event EventHandler? RowsChanged;

    public IReadOnlyList<TableRow> Rows => _rows;

    public void Load(SheetSpec sheet, DateCalendarKind calendar, IEnumerable<TableRow>? initialRows = null)
    {
        _sheet = sheet;
        _calendar = calendar;
        _rows.Clear();

        if (initialRows is not null)
        {
            foreach (var row in initialRows)
            {
                _rows.Add(new TableRow
                {
                    Values = row.Values.ToList(),
                    CreatedAt = row.CreatedAt
                });
            }
        }

        RefreshGrid();
    }

    public void SetCalendar(DateCalendarKind calendar)
    {
        _calendar = calendar;
        RefreshGrid();
    }

    public void ReplaceRows(IEnumerable<TableRow> rows)
    {
        _rows.Clear();
        foreach (var row in rows)
        {
            _rows.Add(new TableRow
            {
                Values = row.Values.ToList(),
                CreatedAt = row.CreatedAt
            });
        }

        RefreshGrid();
    }

    private void RefreshGrid()
    {
        if (_sheet is null || _sheet.Columns.Count == 0)
        {
            RowsGrid.ItemsSource = null;
            _viewTable = null;
            EmptyText.Visibility = Visibility.Visible;
            RowsGrid.Visibility = Visibility.Collapsed;
            UpdateRowButtons();
            return;
        }

        _viewTable = TableRowGridPresenter.BuildViewModel(_sheet, _rows, _calendar);
        RowsGrid.ItemsSource = _viewTable.DefaultView;
        RowsGrid.Height = TableRowGridPresenter.CalculateGridHeight(_rows.Count);
        RowsGrid.MaxHeight = UiMetrics.MaxGridHeight;

        var hasRows = _rows.Count > 0;
        EmptyText.Visibility = hasRows ? Visibility.Collapsed : Visibility.Visible;
        RowsGrid.Visibility = hasRows ? Visibility.Visible : Visibility.Collapsed;
        UpdateRowButtons();
    }

    private void UpdateRowButtons()
    {
        var hasSelection = RowsGrid.SelectedItem is DataRowView;
        EditRowButton.IsEnabled = hasSelection;
        DeleteRowButton.IsEnabled = hasSelection;
    }

    private int? GetSelectedRowIndex()
    {
        if (RowsGrid.SelectedItem is not DataRowView selected)
            return null;

        var index = _viewTable?.Rows.IndexOf(selected.Row) ?? -1;
        return index >= 0 && index < _rows.Count ? index : null;
    }

    private void RowsGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.Column is DataGridBoundColumn boundColumn)
        {
            boundColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            boundColumn.MinWidth = UiMetrics.MinColumnWidth;
        }

        e.Column.Header = e.PropertyName;
    }

    private void RowsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateRowButtons();

    private void AddRow_Click(object sender, RoutedEventArgs e)
    {
        if (_sheet is null)
            return;

        var owner = Window.GetWindow(this);
        var dialog = new AddRowDialog(owner!, _sheet.Columns, _calendar);
        if (dialog.ShowDialog() != true || dialog.ResultRow is null)
            return;

        _rows.Add(TableRow.CreateNew(dialog.ResultRow));
        RowsChanged?.Invoke(this, EventArgs.Empty);
        RefreshGrid();
    }

    private void EditRow_Click(object sender, RoutedEventArgs e)
    {
        var index = GetSelectedRowIndex();
        if (index is null || _sheet is null)
            return;

        var existing = _rows[index.Value];
        var owner = Window.GetWindow(this);
        var dialog = new AddRowDialog(owner!, _sheet.Columns, _calendar, existing.Values, isEdit: true);
        if (dialog.ShowDialog() != true || dialog.ResultRow is null)
            return;

        _rows[index.Value] = TableRow.FromValues(dialog.ResultRow, existing.CreatedAt);
        RowsChanged?.Invoke(this, EventArgs.Empty);
        RefreshGrid();
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
        var index = GetSelectedRowIndex();
        if (index is null)
            return;

        _rows.RemoveAt(index.Value);
        RowsChanged?.Invoke(this, EventArgs.Empty);
        RefreshGrid();
    }
}
