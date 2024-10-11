// Adam Dernis 2024

using Mipser.Services.Localization;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;

namespace Mipser.Windows.Services.Localization;

/// <summary>
/// A service that retrieves localization details.
/// </summary>
public class LocalizationService : ILocalizationService
{
    private ResourceLoader? _loader;

    private ResourceLoader Loader => _loader ??= ResourceLoader.GetForViewIndependentUse();

    /// <inheritdoc/>
    public string this[string key] => Loader.GetString(key);

    /// <inheritdoc/>
    public bool IsRightToLeftLanguage
    {
        get
        {
            string flowDirectionSetting = ResourceContext.GetForCurrentView().QualifierValues["LayoutDirection"];
            return flowDirectionSetting == "RTL";
        }
    }

    /// <inheritdoc/>
    public string LanguageOverride
    {
        get => ApplicationLanguages.PrimaryLanguageOverride;
        set => ApplicationLanguages.PrimaryLanguageOverride = value;
    }

    /// <inheritdoc/>
    public bool IsNeutralLanguage => CultureInfo.CurrentCulture.Name == "en-US";

    /// <inheritdoc/>
    public IReadOnlyList<string> AvailableLanguages => ApplicationLanguages.ManifestLanguages;
}
