using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ExcelCreator.Abstractions;
using ExcelCreator.Composition;
using ExcelCreator.Localization;
using ExcelCreator.Models;
using ExcelCreator.Services;

namespace ExcelCreator;

public partial class MainWindow : Window
{
    private readonly ITemplateRepository _templates;
    private readonly IUserSettingsStore _settings;
    private readonly IAppNavigator _navigator;
    private readonly IAppLogger _logger;
    private List<TemplateDefinition> _allTemplates = [];
    private bool _searchPlaceholderActive = true;

    public MainWindow(
        ITemplateRepository templates,
        IUserSettingsStore settings,
        IAppNavigator navigator,
        IAppLogger logger)
    {
        _templates = templates;
        _settings = settings;
        _navigator = navigator;
        _logger = logger;

        InitializeComponent();
        Title = PersianStrings.MainWindowTitle;
        WelcomeText.Text = PersianStrings.WelcomeTitle;
        SearchBox.Text = PersianStrings.SearchPlaceholder;
        SearchBox.Foreground = new System.Windows.Media.SolidColorBrush(
            System.Windows.Media.Color.FromRgb(0x94, 0xA3, 0xB8));
        SearchBox.LostFocus += SearchBox_LostFocus;
        Loaded += OnLoaded;
    }

    public MainWindow() : this(
        ServiceRegistration.GetRequiredService<ITemplateRepository>(),
        ServiceRegistration.GetRequiredService<IUserSettingsStore>(),
        ServiceRegistration.GetRequiredService<IAppNavigator>(),
        ServiceRegistration.GetRequiredService<IAppLogger>())
    {
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        VersionFooterText.Text = string.Format(PersianStrings.VersionFooterFormat, AppVersion.Informational);
        LoadTemplates();
    }

    private DateCalendarKind DefaultCalendar => _settings.Load().DateCalendar;

    private void SearchBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!SearchBox.IsKeyboardFocusWithin)
            SearchBox.Focus();
        MoveSearchCaretToStart();
        e.Handled = false;
    }

    private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (_searchPlaceholderActive)
        {
            SearchBox.Text = string.Empty;
            SearchBox.Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(0x0F, 0x17, 0x2A));
            _searchPlaceholderActive = false;
        }

        MoveSearchCaretToStart();
    }

    private void MoveSearchCaretToStart()
    {
        SearchBox.Dispatcher.BeginInvoke(() =>
        {
            SearchBox.CaretIndex = 0;
            SearchBox.SelectionLength = 0;
        }, System.Windows.Threading.DispatcherPriority.Input);
    }

    private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            return;
        _searchPlaceholderActive = true;
        SearchBox.Text = PersianStrings.SearchPlaceholder;
        SearchBox.Foreground = new System.Windows.Media.SolidColorBrush(
            System.Windows.Media.Color.FromRgb(0x94, 0xA3, 0xB8));
    }

    private void LoadTemplates()
    {
        try
        {
            _allTemplates = _templates.LoadAll().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to load templates", ex);
            _allTemplates = [];
        }

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = _searchPlaceholderActive ? string.Empty : (SearchBox.Text?.Trim() ?? string.Empty);
        var filtered = string.IsNullOrEmpty(query)
            ? _allTemplates
            : _allTemplates.Where(t =>
                t.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        TemplateList.ItemsSource = filtered;
        EmptyMessage.Text = PersianStrings.NoTemplatesFoundShort;
        EmptyMessage.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

    private void TemplateCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: TemplateDefinition template })
            return;

        var settings = _settings.Load();
        settings.LastTemplateId = template.Id;
        _settings.Save(settings);

        _navigator.ShowTemplateAction(this, template, DefaultCalendar);
    }
}
