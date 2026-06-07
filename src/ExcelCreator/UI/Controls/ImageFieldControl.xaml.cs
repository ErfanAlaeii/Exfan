using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Application.Images;
using ExcelCreator.Localization;

namespace ExcelCreator.UI.Controls;

public partial class ImageFieldControl : UserControl
{
    private readonly IImagePickerService _picker;
    private readonly IImageStorageService _storage;

    public ImageFieldControl(IImagePickerService picker, IImageStorageService storage)
    {
        InitializeComponent();
        _picker = picker;
        _storage = storage;
        PickButton.Content = PersianStrings.ImagePickButton;
        ClearButton.Content = PersianStrings.ImageClearButton;
        SetPath(string.Empty, refreshPreview: false);
    }

    public string SelectedPath { get; private set; } = string.Empty;

    public void SetPath(string? path, bool refreshPreview = true)
    {
        SelectedPath = string.IsNullOrWhiteSpace(path) ? string.Empty : path.Trim();
        if (refreshPreview)
            RefreshPreview();
    }

    private void PickButton_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        if (!_picker.TryPickImage(owner, out var sourcePath))
            return;

        try
        {
            SelectedPath = _storage.ImportFromFile(sourcePath);
            RefreshPreview();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                PersianStrings.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e) => SetPath(string.Empty);

    private void RefreshPreview()
    {
        if (!_storage.Exists(SelectedPath))
        {
            PreviewBorder.Visibility = Visibility.Collapsed;
            PreviewImage.Source = null;
            FileNameText.Text = string.IsNullOrWhiteSpace(SelectedPath)
                ? PersianStrings.ImageNotSelected
                : PersianStrings.ImageFileMissing;
            ClearButton.Visibility = string.IsNullOrWhiteSpace(SelectedPath)
                ? Visibility.Collapsed
                : Visibility.Visible;
            PickButton.Content = string.IsNullOrWhiteSpace(SelectedPath)
                ? PersianStrings.ImagePickButton
                : PersianStrings.ImageChangeButton;
            return;
        }

        var label = _storage.GetDisplayLabel(SelectedPath);
        FileNameText.Text = label;
        PickButton.Content = PersianStrings.ImageChangeButton;
        ClearButton.Visibility = Visibility.Visible;

        try
        {
            if (MediaFileFormats.IsRasterImage(SelectedPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(Path.GetFullPath(SelectedPath));
                bitmap.EndInit();
                bitmap.Freeze();
                PreviewImage.Source = bitmap;
                PreviewBorder.Visibility = Visibility.Visible;
            }
            else
            {
                PreviewBorder.Visibility = Visibility.Collapsed;
                PreviewImage.Source = null;
                FileNameText.Text = MediaFileFormats.FormatGridValue(SelectedPath);
            }
        }
        catch
        {
            PreviewBorder.Visibility = Visibility.Collapsed;
            PreviewImage.Source = null;
        }
    }
}
