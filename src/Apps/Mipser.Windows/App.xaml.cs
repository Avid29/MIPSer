// Adam Dernis 2024

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Mipser.Services.Localization;
using Mipser.Services.Settings;

namespace Mipser.Windows;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Services = ConfigureServices();
        Ioc.Default.ConfigureServices(Services);

        // Apply language override from settings
        var localizationService = Ioc.Default.GetRequiredService<ILocalizationService>();
        var settingsService = Ioc.Default.GetRequiredService<ISettingsService>();
        localizationService.LanguageOverride = settingsService.Local.GetValue<string>("LanguageOverride");

        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window = new MainWindow();
        Window.Activate();
    }
}
