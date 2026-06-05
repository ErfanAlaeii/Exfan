using System.Windows;
using ExcelCreator.Localization;
using ExcelCreator.Models;

namespace ExcelCreator;

public partial class CalculationResultDialog : Window
{
    public CalculationResultDialog(Window owner, CalculationResult result)
    {
        InitializeComponent();
        Owner = owner;
        Title = PersianStrings.CalculationResultDialogTitle;
        CloseButton.Content = PersianStrings.CalculationResultClose;
        TitleText.Text = result.ActionLabel;

        if (!result.IsSuccess)
        {
            ErrorText.Text = result.ErrorMessage ?? PersianStrings.CalculationNotAvailable;
            ErrorText.Visibility = Visibility.Visible;
            ResultsScrollViewer.Visibility = Visibility.Collapsed;
            return;
        }

        ResultItems.ItemsSource = result.Items;

        if (!string.IsNullOrWhiteSpace(result.Summary))
        {
            SummaryText.Text = result.Summary;
            SummaryText.Visibility = Visibility.Visible;
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
