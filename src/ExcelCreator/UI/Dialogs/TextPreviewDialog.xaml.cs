using System.Windows;
using ExcelCreator.Localization;

namespace ExcelCreator.UI.Dialogs;

public partial class TextPreviewDialog : Window
{
    private readonly string _content;

    public TextPreviewDialog(Window owner, string columnHeader, string content)
    {
        InitializeComponent();
        Owner = owner;
        _content = content;
        Title = PersianStrings.TextPreviewDialogTitle;
        TitleText.Text = string.Format(PersianStrings.TextPreviewTitleFormat, columnHeader);
        ContentText.Text = content;
        CopyButton.Content = PersianStrings.CopyText;
        CloseButton.Content = PersianStrings.AboutClose;
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_content))
            return;

        Clipboard.SetText(_content);
        MessageBox.Show(
            PersianStrings.CopyTextSuccess,
            PersianStrings.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
