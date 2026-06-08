using System.Diagnostics;
using System.IO;
using System.Windows;
using ExcelCreator.Application.Images;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Localization;
using ExcelCreator.UI.Dialogs;

namespace ExcelCreator.UI.Presentation;

public sealed class WpfMediaFileOpener : IMediaFileOpener
{
    public bool TryOpen(string? filePath, object? ownerWindow = null)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath.Trim()))
        {
            ShowWarning(PersianStrings.MediaFileMissing, ownerWindow);
            return false;
        }

        var fullPath = Path.GetFullPath(filePath.Trim());
        var owner = ownerWindow as Window;

        if (MediaFileFormats.IsRasterImage(fullPath))
        {
            try
            {
                new MediaPreviewDialog(fullPath) { Owner = owner }.ShowDialog();
                return true;
            }
            catch
            {
                return TryOpenExternally(fullPath, owner);
            }
        }

        return TryOpenExternally(fullPath, owner);
    }

    private static bool TryOpenExternally(string fullPath, Window? owner)
    {
        try
        {
            Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
            return true;
        }
        catch (Exception ex)
        {
            ShowWarning(string.Format(PersianStrings.MediaOpenFailed, ex.Message), owner);
            return false;
        }
    }

    private static void ShowWarning(string message, object? ownerWindow)
    {
        var owner = ownerWindow as Window;
        if (owner is not null)
        {
            MessageBox.Show(owner, message, PersianStrings.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBox.Show(message, PersianStrings.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
