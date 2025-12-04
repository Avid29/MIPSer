// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Services;
using Mipser.Services.Files;
using System.Threading.Tasks;

namespace Mipser.ViewModels;

/// <summary>
/// The main view model for the application.
/// </summary>
public partial class MainViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;
    private readonly IProjectService _projectService;
    private readonly BuildService _buildService;
    private readonly IFileService _fileService;
    private readonly ICacheService _cacheService;

    private PanelViewModel? _focusPanel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel(IMessenger messenger, IProjectService projectService, BuildService buildService, IFileService filesService, ICacheService cacheService)
    {
        _messenger = messenger;
        _projectService = projectService;
        _buildService = buildService;
        _fileService = filesService;
        _cacheService = cacheService;

        // Restore open folder from cache
        _ = RestoreOpenFolder();

        IsActive = true;
    }

    /// <summary>
    /// Gets the currently focused <see cref="PanelViewModel"/>.
    /// </summary>
    public PanelViewModel? FocusedPanel
    {
        get => _focusPanel;
        private set => SetProperty(ref _focusPanel, value);
    }

    private async Task RestoreOpenFolder()
    {
        // Retrieve path from cache
        var openFolderPath = await _cacheService.RetrieveCacheAsync("OpenFolder");
        if (openFolderPath is null)
            return;

        // Retrieve folder
        var folder = await _fileService.GetFolderAsync(openFolderPath);
        if (folder is null)
            return;

        // Open the folder
        _projectService.OpenFolder(folder);
    }
}
