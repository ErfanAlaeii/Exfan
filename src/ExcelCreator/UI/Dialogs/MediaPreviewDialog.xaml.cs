using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ExcelCreator.Localization;

namespace ExcelCreator.UI.Dialogs;

public partial class MediaPreviewDialog : Window
{
    private readonly string _filePath;

    public MediaPreviewDialog(string filePath)
    {
        InitializeComponent();
        _filePath = filePath;
        Title = PersianStrings.MediaPreviewTitle;
        TitleText.Text = Path.GetFileName(filePath);
        OpenExternalButton.Content = PersianStrings.MediaOpenExternal;
        CloseButton.Content = PersianStrings.AboutClose;
        LoadPreview();
    }

    private void LoadPreview()
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(_filePath);
        bitmap.EndInit();
        bitmap.Freeze();
        PreviewImage.Source = bitmap;
    }

    private void OpenExternalButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(_filePath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                string.Format(PersianStrings.MediaOpenFailed, ex.Message),
                PersianStrings.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
