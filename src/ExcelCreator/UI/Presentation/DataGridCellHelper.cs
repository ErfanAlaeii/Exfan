using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExcelCreator.UI.Presentation;

internal static class DataGridCellHelper
{
    public static DataGridCell? FindCell(DependencyObject source)
    {
        while (source is not null and not DataGridCell)
            source = VisualTreeHelper.GetParent(source);

        return source as DataGridCell;
    }

    public static DataGridRow? FindRow(DependencyObject source)
    {
        while (source is not null and not DataGridRow)
            source = VisualTreeHelper.GetParent(source);

        return source as DataGridRow;
    }
}
