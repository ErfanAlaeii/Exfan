using System.Windows;
using ExcelCreator.Localization;

namespace ExcelCreator.UI.Dialogs;

public partial class PersonnelEditDialog : Window
{
    public string? ResultName { get; private set; }

    public PersonnelEditDialog(Window owner, string? initialName = null, bool isEdit = false)
    {
        InitializeComponent();
        Owner = owner;
        Title = isEdit ? PersianStrings.EditPersonnelDialogTitle : PersianStrings.AddPersonnelDialogTitle;
        PromptText.Text = isEdit ? PersianStrings.EditPersonnelDialogTitle : PersianStrings.AddPersonnelDialogTitle;
        NameLabel.Text = PersianStrings.PersonnelNameLabel;
        ConfirmButton.Content = PersianStrings.Confirm;
        CancelButton.Content = PersianStrings.Cancel;
        NameBox.Text = initialName ?? string.Empty;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        var name = NameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(
                PersianStrings.PersonnelNameRequired,
                PersianStrings.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        ResultName = name;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
