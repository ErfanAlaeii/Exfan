using System.Windows;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface IAppNavigator
{
    void ShowTemplateAction(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar);
    void ShowSavedTables(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar);
    void ShowCreateTable(Window owner, TemplateDefinition template, DateCalendarKind defaultCalendar);
    void ShowTableEditor(Window owner, TemplateDefinition template, SavedTable table);
}
