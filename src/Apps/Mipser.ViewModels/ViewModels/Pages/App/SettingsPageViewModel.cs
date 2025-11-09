// Avishai Dernis 2025

using Mipser.Services.Localization;
using Mipser.Services.Settings;
using Mipser.Services.Settings.Enums;
using Mipser.Services.Versioning;
using Mipser.ViewModels.Pages.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mipser.ViewModels.Pages.App;

/// <summary>
/// A view model for the settings page.
/// </summary>
public class SettingsPageViewModel : PageViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingsService _settingsService;
    private readonly IVersioningService _versioningService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    public SettingsPageViewModel(ILocalizationService localizationService, ISettingsService settingsService, IVersioningService versioningService)
    {
        _localizationService = localizationService;
        _settingsService = settingsService;
        _versioningService = versioningService;
    }
    
    /// <inheritdoc/>
    public override string Title => _localizationService["SettingsPageTitle"];

    /// <summary>
    /// Gets or sets the selected app theme.
    /// </summary>
    public Theme AppTheme
    {
        get => _settingsService.Local.GetValue<Theme>(nameof(AppTheme));
        set => _settingsService.Local.SetValue(nameof(AppTheme), value, notify:true);
    }

    /// <summary>
    /// Gets the list of available app theme options.
    /// </summary>
    public IEnumerable<Theme> AppThemeOptions => Enum.GetValues<Theme>();

    /// <summary>
    /// Gets or sets the app language in settings.
    /// </summary>
    public string LanguageOverride
    {
        get => _settingsService.Local.GetValue<string>(nameof(LanguageOverride)) ?? "system";
        set => _settingsService.Local.SetValue(nameof(LanguageOverride), value is "system" ? null : value);
    }

    /// <summary>
    /// Gets the list of available languages in the app.
    /// </summary>
    /// <remarks>
    /// "system" is a sentinel value since null and empty cannot be used in a ComboBox.
    /// </remarks>
    public IEnumerable<string> LanguageOptions => _localizationService.AvailableLanguages.Prepend("system");

    /// <summary>
    /// Gets the app's version.
    /// </summary>
    public string AppVersion =>
        _localizationService["VersionFormat",
            _versioningService.AppVersion.MajorVersion,
            _versioningService.AppVersion.MinorVersion,
            _versioningService.AppVersion.Revision];
}
