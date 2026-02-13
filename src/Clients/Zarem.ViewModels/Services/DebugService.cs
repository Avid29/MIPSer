// Avishai Dernis 2026

using System.Threading.Tasks;
using Zarem.Models.Files;
using Zarem.Services.Popup;
using Zarem.Services.Popup.Enums;
using Zarem.Services.Popup.Models;

namespace Zarem.Services;

/// <summary>
/// A service for handling execution.
/// </summary>
public class DebugService : IDebugService
{
    private readonly IBuildService _buildService;
    private readonly ILocalizationService _localizationService;
    private readonly IPopupService _popupService;
    private readonly IProjectService _projectService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugService"/> class.
    /// </summary>
    public DebugService(IBuildService buildService, ILocalizationService localizationService, IPopupService popupService, IProjectService projectService)
    {
        _buildService = buildService;
        _localizationService = localizationService;
        _popupService = popupService;
        _projectService = projectService;
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task RunAsync(SourceFile file, bool debug = true)
    {
        // TODO: Report issue
        if (_projectService.Project is null)
            return;

        // Check if the file needs to be reassembled
        if (file.IsDirty)
        {
            // Rebuild the file
            await _buildService.AssembleFilesAsync([file]);
        }

        // If the file is still dirty, build failed
        if (file.IsDirty)
        {
            if (file.ObjectFile.Exists)
            {
                // Assembly failed, but an old build exists
                var title = _localizationService["/Popups/FileRunOldAssemblyTitle", file.Name];
                var popup = new PopupDetails(title)
                {
                    Description = _localizationService["/Popups/FileRunOldAssemblyDescription"],
                    PrimaryButtonText = _localizationService["/Popups/FileRunOldAssemblyPrimary"],
                    CloseButtonText = _localizationService["/Popups/Cancel"],
                };

                // Show the popup.
                // Cancel run if closed without primary button click
                var request = await _popupService.ShowPopAsync(popup);
                if (request is PopupResult.Closed)
                    return;
            }
            else
            {
                // Assembly failedd and no old build exists
                var title = _localizationService["/Popups/FileAssemblyFailed", file.Name];
                var popup = new PopupDetails(title)
                {
                    CloseButtonText = _localizationService["/Popups/Okay"],
                };
                
                // Show the popup and return
                await _popupService.ShowPopAsync(popup);
                return;
            }
        }

        // 
    }
}
