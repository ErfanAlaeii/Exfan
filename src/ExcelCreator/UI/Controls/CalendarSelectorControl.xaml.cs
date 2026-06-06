using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Localization;
using ExcelCreator.Core.Models;

namespace ExcelCreator.UI.Controls;

public partial class CalendarSelectorControl : UserControl
{
    private bool _isInitializing;

    public event EventHandler? CalendarChanged;

    public CalendarSelectorControl()
    {
        _isInitializing = true;
        InitializeComponent();
        TitleText.Text = PersianStrings.CalendarSectionTitle;
        JalaliRadio.Content = PersianStrings.CalendarJalali;
        GregorianRadio.Content = PersianStrings.CalendarGregorian;
        JalaliRadio.IsChecked = true;
        UpdateHint();
        _isInitializing = false;
    }

    public DateCalendarKind SelectedCalendar =>
        GregorianRadio.IsChecked == true ? DateCalendarKind.Gregorian : DateCalendarKind.Jalali;

    public void SetCalendar(DateCalendarKind calendar)
    {
        _isInitializing = true;
        if (calendar == DateCalendarKind.Gregorian)
            GregorianRadio.IsChecked = true;
        else
            JalaliRadio.IsChecked = true;

        UpdateHint();
        _isInitializing = false;
    }

    private void CalendarRadio_Changed(object sender, RoutedEventArgs e)
    {
        if (_isInitializing)
            return;

        UpdateHint();
        CalendarChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateHint()
    {
        if (HintText is null)
            return;

        HintText.Text = SelectedCalendar == DateCalendarKind.Gregorian
            ? PersianStrings.DateFormatHintGregorian
            : PersianStrings.DateFormatHintJalali;
    }
}
