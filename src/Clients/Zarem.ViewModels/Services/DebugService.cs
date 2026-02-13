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
    private readonly IPopupService _popupService;
    private readonly IProjectService _projectService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugService"/> class.
    /// </summary>
    public DebugService(IBuildService buildService, IPopupService popupService, IProjectService projectService)
    {
        _buildService = buildService;
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

        // Check if the file needs to be assembled
        if (file.IsDirty)
        {
            // TODO: Localize

            var popup = new PopupDetails("Old.", "Okay?")
            {
                PrimaryButtonText = "New.",
                SecondaryButtonText = "Sure.",
                CloseButtonText = "What?",
            };

            // If the primary button is pressed reassemble
            // If the secondary button is clicked, run with the old binaries
            // If the popup is just closed, cancel the deployment
            var request = await _popupService.ShowPopAsync(popup);
            if (request is PopupResult.Closed)
                return;

            if (request is PopupResult.Primary)
            {
                var result = await _buildService.AssembleFilesAsync([file]);
                if (result is null)
                    return;
            }
        }

    }
}
