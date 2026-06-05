using System.Diagnostics;
using System.IO;
using System.Windows;
using ExcelCreator.Abstractions;
using ExcelCreator.Infrastructure;
using ExcelCreator.Localization;
using ExcelCreator.Models;
using ExcelCreator.Validation;
using Microsoft.Win32;

namespace ExcelCreator.Services;

public sealed class WpfFileExportDialogService : IFileExportDialogService
{
    public bool TryGetSavePath(Window owner, string suggestedFileName, string? initialDirectory, out string filePath)
    {
        var dialog = new SaveFileDialog
        {
            Title = PersianStrings.SaveDialogTitle,
            Filter = PersianStrings.SaveDialogFilter,
            FileName = FileNameSanitizer.Sanitize(suggestedFileName) + ".xlsx",
            DefaultExt = ".xlsx"
        };

        if (!string.IsNullOrWhiteSpace(initialDirectory) && Directory.Exists(initialDirectory))
            dialog.InitialDirectory = initialDirectory;

        if (dialog.ShowDialog() != true)
        {
            filePath = string.Empty;
            return false;
        }

        filePath = dialog.FileName;
        return true;
    }

    public void NotifyExportSuccess(string filePath, bool openAfterCreate)
    {
        if (!openAfterCreate)
            return;

        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });
    }

    public void NotifyError(string message) =>
        MessageBox.Show(
            $"{PersianStrings.CreateFileError}:\n\n{message}",
            PersianStrings.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Error);

    public void NotifyValidationError(string message) =>
        MessageBox.Show(message, PersianStrings.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);

    public void NotifyInfo(string message) =>
        MessageBox.Show(message, PersianStrings.AppName, MessageBoxButton.OK, MessageBoxImage.Information);

    public void NotifyWarning(string message) =>
        MessageBox.Show(message, PersianStrings.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);

    public MessageBoxResult Confirm(string message, MessageBoxButton buttons, MessageBoxImage icon) =>
        MessageBox.Show(message, PersianStrings.AppName, buttons, icon);
}
