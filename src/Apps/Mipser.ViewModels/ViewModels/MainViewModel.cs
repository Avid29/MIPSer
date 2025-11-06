// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Services.Files;
using Mipser.Services.ProjectService;
using Mipser.ViewModels.Pages;
using Mipser.Windows.Services.Cache;

namespace Mipser.ViewModels;

/// <summary>
/// The main view model for the application.
/// </summary>
public partial class MainViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;
    private readonly BuildService _buildService;
    private readonly IFileService _fileService;
    private readonly ICacheService _cacheService;

    private PanelViewModel? _focusPanel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel(IMessenger messenger, BuildService buildService, IFileService filesService, ICacheService cacheService)
    {
        _messenger = messenger;
        _buildService = buildService;
        _fileService = filesService;
        _cacheService = cacheService;

        // Restore open folder from cache
        RestoreOpenFolder();

        IsActive = true;
    }

    /// <summary>
    /// Gets the currently focused <see cref="PanelViewModel"/>.
    /// </summary>
    public PanelViewModel? FocusedPanel
    {
        get => _focusPanel;
        private set
        {
            if (SetProperty(ref _focusPanel, value))
            {
                OnPropertyChanging(nameof(FocusedPanel));
            }
        }
    }

    /// <summary>
    /// Gets the currently open <see cref="FilePageViewModel"/>, or null if the current page is not a file.
    /// </summary>
    public FilePageViewModel? CurrentFilePage => FocusedPanel?.CurrentPage as FilePageViewModel;

    private async void RestoreOpenFolder()
    {
        // Retrieve path from cache
        var openFolderPath = await _cacheService.RetrieveCacheAsync("OpenFolder");
        if (openFolderPath is null)
            return;

        // Retreive folder
        var folder = await _fileService.GetFolderAsync(openFolderPath);
        if (folder is null)
            return;

        // Open the folder
        OpenFolder(folder);
    }
}
