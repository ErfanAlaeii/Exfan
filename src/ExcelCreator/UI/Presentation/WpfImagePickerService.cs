using System.Windows;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Application.Images;
using ExcelCreator.Localization;
using Microsoft.Win32;

namespace ExcelCreator.UI.Presentation;

public sealed class WpfImagePickerService : IImagePickerService
{
    public bool TryPickImage(object? owner, out string sourcePath)
    {
        var dialog = new OpenFileDialog
        {
            Title = PersianStrings.ImagePickerTitle,
            Filter = MediaFileFormats.BuildOpenFileDialogFilter(),
            CheckFileExists = true,
            Multiselect = false
        };

        var result = owner is Window window
            ? dialog.ShowDialog(window)
            : dialog.ShowDialog();

        if (result != true || string.IsNullOrWhiteSpace(dialog.FileName))
        {
            sourcePath = string.Empty;
            return false;
        }

        sourcePath = dialog.FileName;
        return true;
    }
}
