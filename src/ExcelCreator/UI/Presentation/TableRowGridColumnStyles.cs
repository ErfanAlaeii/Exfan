using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ExcelCreator.Core.Models;

namespace ExcelCreator.UI.Presentation;

public static class TableRowGridColumnStyles
{
    public static Style CreateTextCellStyle() =>
        CreateBaseStyle(TextWrapping.NoWrap, TextTrimming.CharacterEllipsis, showAsLink: false);

    public static Style CreateMultilineCellStyle() =>
        CreateBaseStyle(TextWrapping.Wrap, TextTrimming.None, showAsLink: true);

    public static Style CreateMediaCellStyle() =>
        CreateBaseStyle(TextWrapping.NoWrap, TextTrimming.CharacterEllipsis, showAsLink: true);

    public static Style ResolveCellStyle(ColumnSpec? column)
    {
        if (column is null)
            return CreateTextCellStyle();

        if (ColumnTypes.IsImage(column.Type))
            return CreateMediaCellStyle();

        if (ColumnDisplayOptions.IsMultiline(column))
            return CreateMultilineCellStyle();

        return CreateTextCellStyle();
    }

    private static Style CreateBaseStyle(TextWrapping textWrapping, TextTrimming trim, bool showAsLink)
    {
        var style = new Style(typeof(TextBlock));
        style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, textWrapping));
        style.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, trim));
        style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
        style.Setters.Add(new Setter(ToolTipService.ToolTipProperty, new Binding()));

        if (!showAsLink)
            return style;

        style.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(37, 99, 235))));
        style.Setters.Add(new Setter(TextBlock.TextDecorationsProperty, TextDecorations.Underline));
        style.Setters.Add(new Setter(TextBlock.CursorProperty, Cursors.Hand));
        return style;
    }
}
