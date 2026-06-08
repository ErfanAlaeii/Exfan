using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ExcelCreator.Application.Tables;
using ExcelCreator.Composition;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Localization;
using ExcelCreator.UI.Dialogs;
using ExcelCreator.UI.Presentation;
using ExcelCreator.UI.Resources;
using TableRow = ExcelCreator.Core.Models.TableRow;

namespace ExcelCreator.UI.Controls;

public partial class TableRowsEditor : UserControl
{
    private readonly List<TableRow> _rows = [];
    private readonly IMediaFileOpener _mediaFileOpener;
    private readonly IPersonnelRepository _personnel;
    private SheetSpec? _sheet;
    private DateCalendarKind _calendar = DateCalendarKind.Jalali;
    private DataTable? _viewTable;
    private TableRowSearchCriteria? _searchCriteria;
    private IReadOnlyList<int>? _visibleRowIndices;

    public TableRowsEditor()
        : this(
            ServiceRegistration.GetRequiredService<IMediaFileOpener>(),
            ServiceRegistration.GetRequiredService<IPersonnelRepository>())
    {
    }

    internal TableRowsEditor(IMediaFileOpener mediaFileOpener, IPersonnelRepository personnel)
    {
        InitializeComponent();
        _mediaFileOpener = mediaFileOpener;
        _personnel = personnel;
        AddRowButton.Content = PersianStrings.AddRow;
        EditRowButton.Content = PersianStrings.EditRow;
        DeleteRowButton.Content = PersianStrings.DeleteRow;
        SearchRowsButton.Content = PersianStrings.SearchRows;
        FieldSettingsButton.Content = PersianStrings.FieldSettings;
        ClearSearchButton.Content = PersianStrings.ClearSearch;
        EmptyText.Text = PersianStrings.DataEntryEmpty;
        SearchNoResultsText.Text = PersianStrings.SearchNoResults;
    }

    public event EventHandler? RowsChanged;

    public event EventHandler? ColumnsChanged;

    public IReadOnlyList<TableRow> Rows => _rows;

    public SheetSpec? CurrentSheet => _sheet;

    public void Load(SheetSpec sheet, DateCalendarKind calendar, IEnumerable<TableRow>? initialRows = null)
    {
        _sheet = sheet;
        _calendar = calendar;
        _rows.Clear();
        ClearSearchState();

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
            SearchStatusPanel.Visibility = Visibility.Collapsed;
            EmptyText.Visibility = Visibility.Visible;
            RowsGrid.Visibility = Visibility.Collapsed;
            UpdateRowButtons();
            return;
        }

        UpdateVisibleRowIndices();
        var displayRows = GetDisplayRows();
        _viewTable = TableRowGridPresenter.BuildViewModel(_sheet, displayRows, _calendar);
        RowsGrid.ItemsSource = _viewTable.DefaultView;
        ApplyGridLayout();
        UpdateSearchUi();

        if (_rows.Count == 0)
        {
            SearchStatusPanel.Visibility = Visibility.Collapsed;
            EmptyText.Visibility = Visibility.Visible;
            RowsGrid.Visibility = Visibility.Collapsed;
            SearchNoResultsText.Visibility = Visibility.Collapsed;
        }

        UpdateRowButtons();
    }

    private void UpdateVisibleRowIndices()
    {
        if (_searchCriteria is null || _sheet is null)
        {
            _visibleRowIndices = null;
            return;
        }

        _visibleRowIndices = TableRowSearchMatcher.FindMatchingRowIndices(
            _rows,
            _sheet,
            _searchCriteria,
            _calendar);
    }

    private IReadOnlyList<TableRow> GetDisplayRows()
    {
        if (_visibleRowIndices is null)
            return _rows;

        return _visibleRowIndices.Select(index => _rows[index]).ToList();
    }

    private void UpdateSearchUi()
    {
        if (_viewTable is null || _rows.Count == 0)
            return;

        EmptyText.Visibility = Visibility.Collapsed;

        var hasSearch = _searchCriteria is not null;
        var visibleCount = _viewTable.Rows.Count;

        SearchStatusPanel.Visibility = hasSearch ? Visibility.Visible : Visibility.Collapsed;
        ClearSearchButton.Visibility = hasSearch ? Visibility.Visible : Visibility.Collapsed;

        if (hasSearch && _searchCriteria is not null && _sheet is not null)
        {
            var fieldName = GetSearchFieldLabel(_searchCriteria);
            SearchSummaryText.Text = string.Format(
                PersianStrings.SearchSummaryFormat,
                visibleCount,
                _rows.Count,
                fieldName);
        }

        SearchNoResultsText.Visibility = hasSearch && visibleCount == 0
            ? Visibility.Visible
            : Visibility.Collapsed;

        RowsGrid.Visibility = visibleCount > 0 ? Visibility.Visible : Visibility.Collapsed;

        if (_sheet is not null && TableRowGridPresenter.HasMultilineColumns(_sheet))
            RowsGrid.Height = double.NaN;
        else
            RowsGrid.Height = TableRowGridPresenter.CalculateGridHeight(Math.Max(visibleCount, 1));
    }

    private void ApplyGridLayout()
    {
        if (_sheet is null)
            return;

        var multiline = TableRowGridPresenter.HasMultilineColumns(_sheet);
        RowsGrid.RowHeight = multiline ? double.NaN : UiMetrics.GridRowHeight;
        RowsGrid.MaxHeight = multiline ? UiMetrics.MaxMultilineGridHeight : UiMetrics.MaxGridHeight;
    }

    private void ClearSearchState()
    {
        _searchCriteria = null;
        _visibleRowIndices = null;
    }

    private int? MapDisplayIndexToRowIndex(int displayIndex)
    {
        if (displayIndex < 0)
            return null;

        if (_visibleRowIndices is null)
            return displayIndex < _rows.Count ? displayIndex : null;

        return displayIndex < _visibleRowIndices.Count ? _visibleRowIndices[displayIndex] : null;
    }

    private void UpdateRowButtons()
    {
        var hasSelection = RowsGrid.SelectedItem is DataRowView;
        EditRowButton.IsEnabled = hasSelection;
        DeleteRowButton.IsEnabled = hasSelection;
        SearchRowsButton.IsEnabled = _rows.Count > 0;
    }

    private int? GetSelectedRowIndex()
    {
        if (RowsGrid.SelectedItem is not DataRowView selected)
            return null;

        var displayIndex = _viewTable?.Rows.IndexOf(selected.Row) ?? -1;
        return MapDisplayIndexToRowIndex(displayIndex);
    }

    private int? GetRowIndexFromGridRow(DataGridRow gridRow)
    {
        if (gridRow.Item is not DataRowView rowView)
            return null;

        var displayIndex = _viewTable?.Rows.IndexOf(rowView.Row) ?? -1;
        return MapDisplayIndexToRowIndex(displayIndex);
    }

    private void RowsGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.Column is not DataGridBoundColumn boundColumn)
            return;

        var columnSpec = _sheet?.Columns.FirstOrDefault(column => column.Header == e.PropertyName);
        boundColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        boundColumn.MinWidth = columnSpec is not null
            ? TableRowGridPresenter.GetColumnMinWidth(columnSpec)
            : UiMetrics.MinColumnWidth;
        boundColumn.ElementStyle = TableRowGridColumnStyles.ResolveCellStyle(columnSpec);
        e.Column.Header = e.PropertyName;
    }

    private void RowsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        if (_sheet is not null && TableRowGridPresenter.HasMultilineColumns(_sheet))
            e.Row.Height = double.NaN;
    }

    private void RowsGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_sheet is null)
            return;

        if (e.OriginalSource is not DependencyObject source)
            return;

        var cell = DataGridCellHelper.FindCell(source);
        var gridRow = cell is not null
            ? DataGridCellHelper.FindRow(cell)
            : DataGridCellHelper.FindRow(source);

        if (cell is null || gridRow is null)
            return;

        var columnIndex = cell.Column.DisplayIndex;
        if (columnIndex < 0 || columnIndex >= _sheet.Columns.Count)
            return;

        var column = _sheet.Columns[columnIndex];
        var rowIndex = GetRowIndexFromGridRow(gridRow);
        if (rowIndex is null)
            return;

        var cellValue = columnIndex < _rows[rowIndex.Value].Values.Count
            ? _rows[rowIndex.Value].Values[columnIndex]
            : string.Empty;

        if (ColumnTypes.IsImage(column.Type))
        {
            if (string.IsNullOrWhiteSpace(cellValue))
                return;

            _mediaFileOpener.TryOpen(cellValue, Window.GetWindow(this));
            return;
        }

        if (ColumnDisplayOptions.IsMultiline(column))
        {
            if (string.IsNullOrWhiteSpace(cellValue))
                return;

            var owner = Window.GetWindow(this);
            new TextPreviewDialog(owner!, column.Header, cellValue).ShowDialog();
        }
    }

    private void RowsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateRowButtons();

    private void FieldSettings_Click(object sender, RoutedEventArgs e)
    {
        if (_sheet is null)
            return;

        var owner = Window.GetWindow(this);
        var dialog = new FieldSettingsDialog(owner!, _sheet.Columns);
        if (dialog.ShowDialog() != true || dialog.SelectedAction is null)
            return;

        var result = dialog.SelectedAction switch
        {
            TableColumnChangeAction.Add =>
                TableColumnManager.AddColumn(_sheet, dialog.EnteredName ?? string.Empty),
            TableColumnChangeAction.Delete =>
                TableColumnManager.RemoveColumn(_sheet, dialog.SelectedColumnIndex ?? -1, _rows),
            TableColumnChangeAction.Edit =>
                TableColumnManager.RenameColumn(
                    _sheet,
                    dialog.SelectedColumnIndex ?? -1,
                    dialog.EnteredName ?? string.Empty),
            _ => Core.Validation.ValidationResult.Ok()
        };

        if (!result.IsValid)
        {
            MessageBox.Show(
                result.Message,
                PersianStrings.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        TableColumnManager.NormalizeAllRows(_sheet, _rows);
        ClearSearchState();
        RefreshGrid();
        ColumnsChanged?.Invoke(this, EventArgs.Empty);
        RowsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SearchRows_Click(object sender, RoutedEventArgs e)
    {
        if (_sheet is null || _rows.Count == 0)
            return;

        var owner = Window.GetWindow(this);
        var fields = TableRowSearchFields.FromSheet(GetResolvedSheet());
        var dialog = new SearchRowsDialog(owner!, fields, _calendar);
        if (dialog.ShowDialog() != true || dialog.Result is null)
            return;

        _searchCriteria = dialog.Result;
        RefreshGrid();
    }

    private void ClearSearch_Click(object sender, RoutedEventArgs e)
    {
        ClearSearchState();
        RefreshGrid();
    }

    private string GetSearchFieldLabel(TableRowSearchCriteria criteria)
    {
        if (criteria.Target == TableRowSearchTarget.CreatedAt)
            return TableRowMapper.TimestampColumnHeader;

        return _sheet!.Columns[criteria.ColumnIndex].Header;
    }

    private void AddRow_Click(object sender, RoutedEventArgs e)
    {
        if (_sheet is null || !TryPromptIfPersonnelMissing())
            return;

        var owner = Window.GetWindow(this);
        var dialog = new AddRowDialog(owner!, GetResolvedColumns(), _calendar);
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
        var dialog = new AddRowDialog(owner!, GetResolvedColumns(), _calendar, existing.Values, isEdit: true);
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

    private IReadOnlyList<ColumnSpec> GetResolvedColumns() =>
        _sheet is null
            ? []
            : ColumnDropdownResolver.ResolveColumns(_sheet.Columns, _personnel);

    private SheetSpec GetResolvedSheet()
    {
        if (_sheet is null)
            throw new InvalidOperationException("Sheet is not loaded.");

        return new SheetSpec
        {
            Name = _sheet.Name,
            Features = _sheet.Features.ToList(),
            Columns = GetResolvedColumns().ToList()
        };
    }

    private bool TryPromptIfPersonnelMissing()
    {
        if (_sheet is null || !ColumnDropdownResolver.RequiresPersonnel(_sheet.Columns))
            return true;

        if (_personnel.GetNames().Count > 0)
            return true;

        MessageBox.Show(
            PersianStrings.PersonnelListEmpty,
            PersianStrings.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        return false;
    }
}
