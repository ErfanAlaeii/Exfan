using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Abstractions;
using ExcelCreator.Localization;
using ExcelCreator.Models;

namespace ExcelCreator.Controls;

public partial class CalculationActionsControl : UserControl
{
    private ICalculationActionRegistry? _registry;
    private ICalculationEngine? _engine;
    private Func<CalculationContext>? _contextFactory;

    public CalculationActionsControl()
    {
        InitializeComponent();
        SectionTitleText.Text = PersianStrings.CalculationsSectionTitle;
        SectionHintText.Text = PersianStrings.CalculationsSectionHint;
        NoActionsText.Text = PersianStrings.CalculationNotAvailable;
    }

    public void Initialize(
        ICalculationActionRegistry registry,
        ICalculationEngine engine,
        Func<CalculationContext> contextFactory)
    {
        _registry = registry;
        _engine = engine;
        _contextFactory = contextFactory;
        RefreshActions();
    }

    public void RefreshActions()
    {
        if (_registry is null || _engine is null || _contextFactory is null)
        {
            ActionsItems.ItemsSource = null;
            NoActionsText.Visibility = Visibility.Visible;
            return;
        }

        ActionsItems.ItemsSource = _registry.GetAll();
        NoActionsText.Visibility = Visibility.Collapsed;
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not string actionId ||
            _engine is null ||
            _contextFactory is null)
        {
            return;
        }

        var owner = Window.GetWindow(this);
        var context = _contextFactory();
        var action = _registry!.GetById(actionId);
        CalculationParameters? parameters = null;

        if (action is not null && action.InputKind != CalculationInputKind.None)
        {
            var picker = new CalculationColumnPickerDialog(owner!, context.Sheet, action.InputKind);
            if (picker.ShowDialog() != true || !picker.IsValid)
                return;

            parameters = picker.Parameters;
        }

        var result = _engine.Execute(actionId, context, parameters);
        var dialog = new CalculationResultDialog(owner!, result);
        dialog.ShowDialog();
    }
}
