// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Services.Build;
using Mipser.Services.Files;
using Mipser.Windows.Services.Cache;

namespace Mipser.Services.Project;

/// <summary>
/// An implementation of the <see cref="IProjectService"/> interface.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IMessenger _messenger;
    private readonly ICacheService _cacheService;
    private readonly IFileService _fileService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectService"/> class.
    /// </summary>
    public ProjectService(IMessenger messenger, ICacheService cacheService, IFileService fileService)
    {
        _messenger = messenger;
        _cacheService = cacheService;
        _fileService = fileService;

        RestoreOpenFolder();
    }
    
    /// <inheritdoc/>
    public BindableFolder? ProjectRootFolder { get; private set; }

    /// <inheritdoc/>
    public void OpenFolder(BindableFolder folder)
    {
        // Change the root folder
        // Send a message notifying the change
        ProjectRootFolder = folder;
        _messenger.Send(new FolderOpenedMessage(folder));

        // Cache the open folder.
        _ = _cacheService.CacheAsync("OpenFolder", folder.Path);

    }

    private async void RestoreOpenFolder()
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
        OpenFolder(folder);
    }
}
