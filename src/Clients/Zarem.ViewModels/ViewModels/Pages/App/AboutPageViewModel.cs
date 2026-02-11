// Avishai Dernis 2025

using Zarem.Services;
using Zarem.ViewModels.Pages.Abstract;

namespace Zarem.ViewModels.Pages.App;

/// <summary>
/// A view model for the about page.
/// </summary>
public class AboutPageViewModel : PageViewModel
{
    private ILocalizationService _localizationService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutPageViewModel"/> class.
    /// </summary>
    public AboutPageViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    /// <inheritdoc/>
    public override string Title => _localizationService["/PageTitles/About"];
}
