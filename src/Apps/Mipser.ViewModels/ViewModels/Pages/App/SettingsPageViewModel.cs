// Avishai Dernis 2025

using Mipser.Services;
using Mipser.Services.Files;
using Mipser.Services.Settings;
using Mipser.Services.Versioning;
using Mipser.ViewModels.Pages.Abstract;
using Mipser.ViewModels.Pages.App.Settings;
using System.Collections.ObjectModel;

namespace Mipser.ViewModels.Pages.App;

/// <summary>
/// A view model for the settings page.
/// </summary>
public class SettingsPageViewModel : PageViewModel
{
    private readonly ILocalizationService _localizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    public SettingsPageViewModel(ILocalizationService localizationService, ISettingsService settingsService, IFileSystemService fileSystemService, IVersioningService versioningService)
    {
        _localizationService = localizationService;

        SubPages = [
                new AppSettingsViewModel(localizationService, settingsService, versioningService),
                new EditorSettingsViewModel(localizationService, settingsService, fileSystemService),
                new AssemblerSettingsViewModel(localizationService, settingsService)
            ];
    }

    /// <inheritdoc/>
    public override string Title => _localizationService["/PageTitles/Settings"];

    /// <summary>
    /// Gets the collection of settings sub-pages.
    /// </summary>
    public ObservableCollection<SettingsSubPageViewModel> SubPages { get; }
}
