using System.Windows;
using ExcelCreator.Composition;
using FluentAssertions;

namespace ExcelCreator.Tests;

public class StartupSmokeTests
{
    [Fact]
    public void ServiceRegistration_ResolvesAllSingletons()
    {
        RunOnSta(() =>
        {
            ServiceRegistration.Configure();

            ServiceRegistration.GetRequiredService<ExcelCreator.Abstractions.ITemplateRepository>().Should().NotBeNull();
            ServiceRegistration.GetRequiredService<ExcelCreator.Abstractions.IAppNavigator>().Should().NotBeNull();
        });
    }

    [Fact]
    public void MainWindow_CanBeConstructed()
    {
        Exception? failure = null;

        RunOnSta(() =>
        {
            try
            {
                if (Application.Current is null)
                {
                    var app = new ExcelCreator.App();
                    app.InitializeComponent();
                }

                ServiceRegistration.Configure();
                _ = new ExcelCreator.MainWindow();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });

        failure.Should().BeNull($"MainWindow construction failed: {failure}");
    }

    private static void RunOnSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null)
            throw failure;
    }
}
