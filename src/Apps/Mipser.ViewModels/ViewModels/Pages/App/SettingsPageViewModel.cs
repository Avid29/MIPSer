// Avishai Dernis 2025

using Mipser.Services.Localization;
using Mipser.Services.Settings;
using Mipser.ViewModels.Pages.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Mipser.ViewModels.Pages.App;

/// <summary>
/// A view model for the settings page.
/// </summary>
public class SettingsPageViewModel : PageViewModel
{
    private ILocalizationService _localizationService;
    private ISettingsService _settingsService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    public SettingsPageViewModel(ILocalizationService localizationService, ISettingsService settingsService)
    {
        _localizationService = localizationService;
        _settingsService = settingsService;
    }
    
    /// <inheritdoc/>
    public override string Title => _localizationService["SettingsPageTitle"];

    /// <summary>
    /// Gets or sets the 
    /// </summary>
    public string Language
    {
        get => _settingsService.Local.GetValue<string>("LanguageOverride") ?? "system";
        set => _settingsService.Local.SetValue("LanguageOverride", value is "system" ? null : value);
    }

    /// <summary>
    /// Gets the list of available languages in the app.
    /// </summary>
    /// <remarks>
    /// "system" is a sentinel value since null and empty cannot be used in a ComboBox.
    /// </remarks>
    public IEnumerable<string> LanguageOptions => _localizationService.AvailableLanguages.Prepend("system");
}
