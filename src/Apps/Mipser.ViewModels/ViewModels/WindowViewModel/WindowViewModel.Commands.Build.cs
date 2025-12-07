// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Build;
using Mipser.Messages.Files;
using Mipser.Messages.Pages;
using Mipser.Services;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.App;
using System.Threading.Tasks;

namespace Mipser.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets a command that builds the project.
    /// </summary>
    public AsyncRelayCommand BuildProjectCommand { get; }

    /// <summary>
    /// Gets a command that assembles the current file.
    /// </summary>
    public AsyncRelayCommand AssembleFileCommand { get; }

    private async Task BuildProjectAsync()
    {
        
    }

    private async Task AssembleFileAsync()
    {
        // Get the current page, and ensure it's a file page
        if (MainViewModel.FocusedPanel?.CurrentPage is not FilePageViewModel page)
            return;

        // Check if the file is null
        var file = page.File?.SourceFile;
        if (file is null)
            return;

        // Request to assemble the file
        await _buildService.AssembleFilesAsync([file]);
    }
}
