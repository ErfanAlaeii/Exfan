using System.Windows;
using System.Windows.Input;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Localization;
using ExcelCreator.Core.Models;

namespace ExcelCreator.UI.Windows;

public partial class TemplateActionWindow : Window
{
    private readonly TemplateDefinition _template;
    private readonly DateCalendarKind _defaultCalendar;
    private readonly IAppNavigator _navigator;

    public TemplateActionWindow(
        TemplateDefinition template,
        DateCalendarKind defaultCalendar,
        IAppNavigator navigator)
    {
        InitializeComponent();
        _template = template;
        _defaultCalendar = defaultCalendar;
        _navigator = navigator;

        Title = PersianStrings.TemplateActionWindowTitle;
        TemplateTitleText.Text = template.Title;
        ActionPromptText.Text = PersianStrings.TemplateActionPrompt;
        ViewExistingTitle.Text = PersianStrings.ViewExistingTables;
        ViewExistingHint.Text = PersianStrings.ViewExistingTablesHint;
        CreateNewTitle.Text = PersianStrings.CreateNewTable;
        CreateNewHint.Text = PersianStrings.CreateNewTableHint;
        CancelButton.Content = PersianStrings.Cancel;
    }

    private void ViewExisting_Click(object sender, MouseButtonEventArgs e)
    {
        Close();
        _navigator.ShowSavedTables(Owner, _template, _defaultCalendar);
    }

    private void CreateNew_Click(object sender, MouseButtonEventArgs e)
    {
        Close();
        _navigator.ShowCreateTable(Owner, _template, _defaultCalendar);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
