using System.Windows;
using System.Windows.Controls;
using ExcelCreator.Application.Common;
using ExcelCreator.Core.Models;

namespace ExcelCreator.UI.Controls;

public partial class CalendarDateInputControl : UserControl
{
    private DateCalendarKind _calendar = DateCalendarKind.Jalali;
    private DatePicker? _gregorianPicker;
    private JalaliDatePickerControl? _jalaliPicker;

    public CalendarDateInputControl()
    {
        InitializeComponent();
        ApplyCalendar();
    }

    public void SetCalendar(DateCalendarKind calendar)
    {
        if (_calendar == calendar && InputHost.Content is not null)
            return;

        _calendar = calendar;
        ApplyCalendar();
    }

    public string GetSearchValue()
    {
        if (_calendar == DateCalendarKind.Gregorian)
        {
            return _gregorianPicker?.SelectedDate is DateTime date
                ? DateCalendarService.FormatGregorianSearchValue(date)
                : string.Empty;
        }

        return _jalaliPicker?.GetGregorianSearchValue() ?? string.Empty;
    }

    private void ApplyCalendar()
    {
        if (_calendar == DateCalendarKind.Gregorian)
        {
            _gregorianPicker ??= new DatePicker
            {
                FontSize = 14,
                Padding = new Thickness(8, 6, 8, 6),
                FlowDirection = FlowDirection.RightToLeft,
                SelectedDate = DateTime.Today
            };
            InputHost.Content = _gregorianPicker;
            return;
        }

        _jalaliPicker ??= new JalaliDatePickerControl();
        InputHost.Content = _jalaliPicker;
    }
}
