namespace ExcelCreator.Core.Models;

public sealed class AppSettings
{
    public string? DefaultSaveFolder { get; set; }
    public bool OpenAfterCreate { get; set; } = true;
    public string? LastTemplateId { get; set; }
    public DateCalendarKind DateCalendar { get; set; } = DateCalendarKind.Jalali;
}
