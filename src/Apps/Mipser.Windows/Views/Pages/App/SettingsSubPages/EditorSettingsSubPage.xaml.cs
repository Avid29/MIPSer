// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Mipser.Messages;
using Mipser.Models.EditorConfig.ColorScheme;
using Mipser.Services;
using Mipser.Services.Settings.Enums;
using Mipser.ViewModels.Pages.App.Settings;

namespace Mipser.Windows.Views.Pages.App.SettingsSubPages;

/// <summary>
/// The app settings subpage
/// </summary>
public sealed partial class EditorSettingsSubPage : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditorSettingsSubPage"/> class.
    /// </summary>
    public EditorSettingsSubPage()
    {
        this.InitializeComponent();

        Ioc.Default.GetRequiredService<IMessenger>().Register<EditorSettingsSubPage, SettingChangedMessage<Theme>>(this, (r, m) => SyntaxHighlighting.ReloadFromSettings());
        Ioc.Default.GetRequiredService<IMessenger>().Register<EditorSettingsSubPage, SettingChangedMessage<EditorColorScheme>>(this, (r, m) => SyntaxHighlighting.ReloadFromSettings());
    }

    public EditorSettingsViewModel? ViewModel { get; set; }

    public string DemoText = Ioc.Default.GetRequiredService<ILocalizationService>()["/Settings/EditorDemoText"];
}
