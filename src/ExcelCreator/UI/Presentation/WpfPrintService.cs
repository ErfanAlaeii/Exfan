using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Localization;
using DocTableRow = System.Windows.Documents.TableRow;

namespace ExcelCreator.UI.Presentation;

public sealed class WpfPrintService : IPrintService
{
    public bool Print(TablePrintModel model, object? ownerWindow = null)
    {
        var document = BuildDocument(model);
        var dialog = new PrintDialog();

        if (dialog.ShowDialog() != true)
            return false;

        document.PageHeight = dialog.PrintableAreaHeight;
        document.PageWidth = dialog.PrintableAreaWidth;
        document.ColumnWidth = dialog.PrintableAreaWidth;

        var jobTitle = string.IsNullOrWhiteSpace(model.Title)
            ? PersianStrings.AppName
            : model.Title;

        dialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, jobTitle);
        return true;
    }

    private static FlowDocument BuildDocument(TablePrintModel model)
    {
        var document = new FlowDocument
        {
            FlowDirection = FlowDirection.RightToLeft,
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 12,
            PagePadding = new Thickness(48)
        };

        document.Blocks.Add(new Paragraph(new Run(model.Title))
        {
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Right,
            Margin = new Thickness(0, 0, 0, 16)
        });

        var table = new Table { CellSpacing = 0 };
        foreach (var _ in model.Headers)
            table.Columns.Add(new TableColumn());

        var rowGroup = new TableRowGroup();
        rowGroup.Rows.Add(CreateHeaderRow(model.Headers));
        foreach (var row in model.Rows)
            rowGroup.Rows.Add(CreateDataRow(row));

        table.RowGroups.Add(rowGroup);
        document.Blocks.Add(table);
        return document;
    }

    private static DocTableRow CreateHeaderRow(IReadOnlyList<string> headers)
    {
        var row = new DocTableRow { Background = Brushes.Gainsboro, FontWeight = FontWeights.SemiBold };
        foreach (var header in headers)
            row.Cells.Add(CreateCell(header, isHeader: true));

        return row;
    }

    private static DocTableRow CreateDataRow(IReadOnlyList<string> values)
    {
        var row = new DocTableRow();
        foreach (var value in values)
            row.Cells.Add(CreateCell(value, isHeader: false));

        return row;
    }

    private static TableCell CreateCell(string text, bool isHeader)
    {
        return new TableCell(new Paragraph(new Run(text)))
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0.5),
            Padding = new Thickness(6, 4, 6, 4),
            TextAlignment = TextAlignment.Right
        };
    }
}
