using System.Windows;
using System.Windows.Input;
using ExcelCreator.Composition;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Localization;
using ExcelCreator.UI.Dialogs;

namespace ExcelCreator.UI.Windows;

public partial class PersonnelWindow : Window
{
    private readonly IPersonnelRepository _personnel;
    private readonly IFileExportDialogService _dialogs;
    private readonly IAppLogger _logger;
    private List<PersonnelMember> _items = [];

    public PersonnelWindow(
        IPersonnelRepository personnel,
        IFileExportDialogService dialogs,
        IAppLogger logger)
    {
        InitializeComponent();
        _personnel = personnel;
        _dialogs = dialogs;
        _logger = logger;

        Title = PersianStrings.PersonnelWindowTitle;
        HeaderText.Text = PersianStrings.PersonnelHeader;
        ListHintText.Text = PersianStrings.PersonnelHint;
        EmptyText.Text = PersianStrings.PersonnelEmpty;
        CloseButton.Content = PersianStrings.AboutClose;
        DeleteButton.Content = PersianStrings.DeletePersonnel;
        EditButton.Content = PersianStrings.EditPersonnel;
        AddButton.Content = PersianStrings.AddPersonnel;
        LoadPersonnel();
    }

    public PersonnelWindow()
        : this(
            ServiceRegistration.GetRequiredService<IPersonnelRepository>(),
            ServiceRegistration.GetRequiredService<IFileExportDialogService>(),
            ServiceRegistration.GetRequiredService<IAppLogger>())
    {
    }

    private void LoadPersonnel()
    {
        try
        {
            _items = _personnel.GetAll().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to load personnel", ex);
            _dialogs.NotifyError(ex.Message);
            _items = [];
        }

        PersonnelList.ItemsSource = _items;
        EmptyText.Visibility = _items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PersonnelList.Visibility = _items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private PersonnelMember? GetSelectedMember() =>
        PersonnelList.SelectedItem as PersonnelMember;

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new PersonnelEditDialog(this);
        if (dialog.ShowDialog() != true || dialog.ResultName is null)
            return;

        try
        {
            _personnel.Add(dialog.ResultName);
            LoadPersonnel();
        }
        catch (Exception ex)
        {
            _dialogs.NotifyError(ex.Message);
        }
    }

    private void Edit_Click(object sender, RoutedEventArgs e) => EditSelected();

    private void PersonnelList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => EditSelected();

    private void EditSelected()
    {
        var member = GetSelectedMember();
        if (member is null)
        {
            _dialogs.NotifyInfo(PersianStrings.SelectPersonnelPrompt);
            return;
        }

        var dialog = new PersonnelEditDialog(this, member.Name, isEdit: true);
        if (dialog.ShowDialog() != true || dialog.ResultName is null)
            return;

        try
        {
            _personnel.Update(member.Id, dialog.ResultName);
            LoadPersonnel();
        }
        catch (Exception ex)
        {
            _dialogs.NotifyError(ex.Message);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var member = GetSelectedMember();
        if (member is null)
        {
            _dialogs.NotifyInfo(PersianStrings.SelectPersonnelPrompt);
            return;
        }

        if (_dialogs.Confirm(
                string.Format(PersianStrings.DeletePersonnelConfirm, member.Name),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        try
        {
            _personnel.Delete(member.Id);
            LoadPersonnel();
        }
        catch (Exception ex)
        {
            _dialogs.NotifyError(ex.Message);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
