// Avishai Dernis 2025

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Mipser.Services.Localization;
using Mipser.ViewModels.Pages.App;
using System.Globalization;

namespace Mipser.Windows.Views.Pages.App;

/// <summary>
/// A viewer for files.
/// </summary>
public sealed partial class SettingsPage : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileViewer"/> class.
    /// </summary>
    public SettingsPage()
    {
        this.InitializeComponent();
    }

    public SettingsPageViewModel? ViewModel { get; set; }

    public static string? LanguageDisplayName(string? code)
    {
        if (code is null)
            return null;

        // "system" is a sentinel value since null and empty cannot be used in a ComboBox
        if (code is "system")
        {
            var localization = Ioc.Default.GetRequiredService<ILocalizationService>();
            return localization["UseSystemLanguage"];
        }

        var target = new CultureInfo(code);
        var nativeName = target.NativeName;
        var displayName = target.DisplayName;

        // Avoid displaying redundant info
        if (nativeName == displayName)
            return nativeName;

        // 
        return $"{nativeName} - {displayName}";
    }
}
