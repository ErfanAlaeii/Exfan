using System.Windows;
using ExcelCreator.Models;

namespace ExcelCreator.Abstractions;

public interface IAppNavigator
{
    void ShowTemplateAction(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar);
    void ShowSavedTables(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar);
    void ShowCreateTable(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar);
    void ShowTableEditor(Window owner, TemplateDefinition template, SavedTable table);
}
