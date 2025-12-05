// Adam Dernis 2024

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Mipser.Messages;
using Mipser.Services;
using Mipser.Services.Settings;
using Mipser.Services.Settings.Enums;

namespace Mipser.Windows;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private readonly IMessenger _messenger;
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // Setup services
        Services = ConfigureServices();
        Ioc.Default.ConfigureServices(Services);

        // Track necessary services
        _messenger = Service.Get<IMessenger>();
        _settingsService = Service.Get<ISettingsService>();

        // Apply language override from settings
        var localizationService = Service.Get<ILocalizationService>();
        localizationService.LanguageOverride = _settingsService.Local.GetValue<string>(SettingsKeys.LanguageOverride);

        // Subscribe to messages
        _messenger.Register<App, SettingChangedMessage<Theme>>(this, (r, m) => r.ApplyRequestedTheme(m.NewValue));

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

        // Apply theme settings
        LoadRequestedTheme();

        // Initiaize dispatcher on UI thread.
        Services.GetRequiredService<IDispatcherService>().Init();
    }

    private void LoadRequestedTheme()
    {
        var theme = _settingsService.Local.GetValue<Theme>(SettingsKeys.AppTheme);
        ApplyRequestedTheme(theme);
    }

    private void ApplyRequestedTheme(Theme theme = default)
    {
        // Find root item
        var root = Window?.Content.FindDescendantOrSelf<FrameworkElement>();
        if(root is null)
            return;

        // Update theme from root content
        root.RequestedTheme = theme switch
        {
            Theme.Dark => ElementTheme.Dark,
            Theme.Light => ElementTheme.Light,
            Theme.Default or _ => ElementTheme.Default,
        };
    }
}
