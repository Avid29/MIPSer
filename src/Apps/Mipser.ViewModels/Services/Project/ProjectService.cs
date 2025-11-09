// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Services.Build;
using Mipser.Services.Files;
using Mipser.Windows.Services.Cache;
using System.Threading.Tasks;

namespace Mipser.Services.Project;

/// <summary>
/// An implementation of the <see cref="IProjectService"/> interface.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IMessenger _messenger;
    private readonly ICacheService _cacheService;
    private readonly IFileService _fileService;
    
    private BindableFolder? _objFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectService"/> class.
    /// </summary>
    public ProjectService(IMessenger messenger, ICacheService cacheService, IFileService fileService)
    {
        _messenger = messenger;
        _cacheService = cacheService;
        _fileService = fileService;

        _ = RestoreOpenFolder();
    }
    
    /// <inheritdoc/>
    public BindableFolder? ProjectRootFolder { get; private set; }

    /// <inheritdoc/>
    public void OpenFolder(BindableFolder folder)
    {
        // Clear the cached obj folder
        _objFolder = null;

        // Change the root folder
        ProjectRootFolder = folder;

        // Send a message notifying the change
        _messenger.Send(new FolderOpenedMessage(folder));

        // Cache the open folder.
        _ = _cacheService.CacheAsync("OpenFolder", folder.Path);
    }
    
    /// <inheritdoc/>
    public async Task<BindableFolder?> GetObjectFolderAsync()
    {
        if (_objFolder is not null)
            return _objFolder;

        if (ProjectRootFolder is null)
            return null;

        var objFolder = await ProjectRootFolder.OpenFolderAsync("obj");
        objFolder ??= await ProjectRootFolder.CreateFolderAsync("obj");

        _objFolder = objFolder;
        return objFolder;
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
        OpenFolder(folder);
    }
}
