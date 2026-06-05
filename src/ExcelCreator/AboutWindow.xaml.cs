using System.Reflection;
using System.Windows;
using ExcelCreator.Services;

namespace ExcelCreator;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        CloseButton.Content = Localization.PersianStrings.AboutClose;
        Title = Localization.PersianStrings.AboutWindowTitle;
        ProductNameText.Text = AppVersion.Product;
        VersionText.Text = $"نسخه {AppVersion.Informational}";
        CopyrightText.Text = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
