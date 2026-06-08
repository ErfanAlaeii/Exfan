using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ExcelCreator.Application.Common;
using ExcelCreator.Core.Models;
using ExcelCreator.Localization;

namespace ExcelCreator.UI.Controls;

public partial class JalaliDatePickerControl : UserControl
{
    private readonly Button[,] _dayButtons = new Button[6, 7];
    private DateTime _selectedDate = DateTime.Today;
    private int _displayYear;
    private int _displayMonth;

    public JalaliDatePickerControl()
    {
        InitializeComponent();
        DateCalendarService.GetJalaliParts(_selectedDate, out _displayYear, out _displayMonth, out _);
        BuildWeekdayHeaders();
        BuildDayButtons();
        CalendarPopup.Closed += (_, _) => DropDownButton.IsChecked = false;
        RenderCalendar();
        UpdateDisplayText();
    }

    public string GetGregorianSearchValue() =>
        DateCalendarService.FormatGregorianSearchValue(_selectedDate);

    private void BuildWeekdayHeaders()
    {
        foreach (var weekday in JalaliCalendarLabels.WeekdayNames)
        {
            WeekdayHeaderGrid.Children.Add(new TextBlock
            {
                Text = weekday,
                TextAlignment = TextAlignment.Center,
                Foreground = (Brush)FindResource("MutedBrush"),
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 4)
            });
        }
    }

    private void BuildDayButtons()
    {
        for (var row = 0; row < 6; row++)
        {
            for (var column = 0; column < 7; column++)
            {
                var button = new Button
                {
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(2),
                    Padding = new Thickness(0),
                    FontSize = 13,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Visibility = Visibility.Hidden
                };
                button.Click += DayButton_Click;
                _dayButtons[row, column] = button;
                DayGrid.Children.Add(button);
            }
        }
    }

    private void RenderCalendar()
    {
        MonthYearText.Text = $"{JalaliCalendarLabels.MonthNames[_displayMonth - 1]} {DateCalendarService.ToPersianDigits(_displayYear.ToString(CultureInfo.InvariantCulture))}";

        foreach (var button in _dayButtons)
        {
            button.Visibility = Visibility.Hidden;
            button.IsEnabled = false;
            button.Content = null;
            button.Background = Brushes.Transparent;
            button.Foreground = (Brush)FindResource("TextBrush");
            button.FontWeight = FontWeights.Normal;
            button.BorderThickness = new Thickness(0);
        }

        if (!DateCalendarService.TryFromJalaliParts(_displayYear, _displayMonth, 1, out var firstDay))
            return;

        var startColumn = DateCalendarService.GetJalaliGridColumn(firstDay.DayOfWeek);
        var daysInMonth = DateCalendarService.GetJalaliDaysInMonth(_displayYear, _displayMonth);
        DateCalendarService.GetJalaliParts(DateTime.Today, out var todayYear, out var todayMonth, out var todayDay);
        DateCalendarService.GetJalaliParts(_selectedDate, out var selectedYear, out var selectedMonth, out var selectedDay);

        for (var day = 1; day <= daysInMonth; day++)
        {
            var cellIndex = startColumn + day - 1;
            var row = cellIndex / 7;
            var column = cellIndex % 7;
            if (row >= 6)
                break;

            var button = _dayButtons[row, column];
            button.Visibility = Visibility.Visible;
            button.IsEnabled = true;
            button.Content = DateCalendarService.ToPersianDigits(day.ToString(CultureInfo.InvariantCulture));
            button.Tag = day;

            if (day == selectedDay && _displayYear == selectedYear && _displayMonth == selectedMonth)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                button.Foreground = Brushes.White;
                button.FontWeight = FontWeights.SemiBold;
            }
            else if (day == todayDay && _displayYear == todayYear && _displayMonth == todayMonth)
            {
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                button.BorderThickness = new Thickness(1);
            }
        }
    }

    private void UpdateDisplayText()
    {
        DisplayBox.Text = DateCalendarService.Format(_selectedDate, DateCalendarKind.Jalali);
    }

    private void DayButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: int day })
            return;

        if (!DateCalendarService.TryFromJalaliParts(_displayYear, _displayMonth, day, out var selected))
            return;

        _selectedDate = selected;
        UpdateDisplayText();
        RenderCalendar();
        CalendarPopup.IsOpen = false;
        DropDownButton.IsChecked = false;
    }

    private void PreviousMonthButton_Click(object sender, RoutedEventArgs e)
    {
        if (_displayMonth == 1)
        {
            _displayMonth = 12;
            _displayYear--;
        }
        else
        {
            _displayMonth--;
        }

        RenderCalendar();
    }

    private void NextMonthButton_Click(object sender, RoutedEventArgs e)
    {
        if (_displayMonth == 12)
        {
            _displayMonth = 1;
            _displayYear++;
        }
        else
        {
            _displayMonth++;
        }

        RenderCalendar();
    }

    private void DropDownButton_Click(object sender, RoutedEventArgs e) =>
        CalendarPopup.IsOpen = DropDownButton.IsChecked == true;
}
