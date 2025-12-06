// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Models.ProjectConfig;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using Mipser.Services.Settings;
using System.Threading.Tasks;

namespace Mipser.Services;

/// <summary>
/// An implementation of the <see cref="IProjectService"/> interface.
/// </summary>
public class ProjectService : IProjectService
{
    private const string OpenProjectCacheKey = "OpenProject";
    private const string OpenFolderCacheKey = "OpenFolder";

    private readonly IMessenger _messenger;
    private readonly ICacheService _cacheService;
    private readonly IFileSystemService _fileSystemService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectService"/> class.
    /// </summary>
    public ProjectService(IMessenger messenger, ISettingsService settingsService, ICacheService cacheService, IFileSystemService fileSystemService)
    {
        _messenger = messenger;
        _cacheService = cacheService;
        _fileSystemService = fileSystemService;

        if (settingsService.Local.GetValue<bool>(SettingsKeys.RestoreOpenProject))
        {
            _ = RestoreOpenProject();
        }
    }

    /// <inheritdoc/>
    public Project? Project { get; private set; }

    /// <inheritdoc/>
    public IFolder? ProjectRootFolder { get; private set; }

    /// <inheritdoc/>
    public void OpenFolder(IFolder? folder, bool cacheState = true)
    {
        // Change the root folder
        ProjectRootFolder = folder;

        // Send a message notifying the change
        _messenger.Send(new FolderOpenedMessage(folder));

        // Update the state.
        if (cacheState)
        {
            _ = CacheOpenProjectAsync(true);
        }
    }

    /// <inheritdoc/>
    public async Task OpenFolderAsync(string path, bool cacheState = true)
    {
        // Load the folder
        var folder = await _fileSystemService.GetFolderAsync(path);
        if (folder is null)
            return;

        // Open the folder
        OpenFolder(folder, cacheState);
    }

    /// <inheritdoc/>
    public async Task OpenProjectAsync(ProjectConfig config, bool cacheState = true)
    {
        Project = Project.Load(config);
        if (Project?.Config?.RootFolderPath is null)
            return;

        await OpenFolderAsync(Project.Config.RootFolderPath, false);

        if (cacheState)
        {
            await CacheOpenProjectAsync();
        }
    }

    /// <inheritdoc/>
    public async Task OpenProjectAsync(string path, bool cacheState = true)
    {
        // Attempt to load the file
        var file = await _fileSystemService.GetFileAsync(path);
        if (file is null)
            return;

        // Attempt to open the file as a stream
        var stream = await file.OpenStreamForReadAsync();
        if (stream is null)
            return;

        // Attempt to deserialize
        var config = await ProjectConfig.DeserializeAsync(path, stream);
        if (config is null)
            return;

        // Open the project
        await OpenProjectAsync(config, cacheState);
    }

    /// <inheritdoc/>
    public async Task CloseProjectAsync()
    {
        Project = null;
        OpenFolder(null, false);

        await ClearOpenCacheAsync();
    }

    private async Task CacheOpenProjectAsync(bool folder = false)
    {
        // Clear current cache
        await ClearOpenCacheAsync();

        // Cache value and key
        var (key, value) = folder switch
        {
            false => (OpenProjectCacheKey, Project?.Config?.ConfigPath),
            true => (OpenFolderCacheKey, ProjectRootFolder?.Path),
        };
        await _cacheService.CacheAsync(key, value);
    }

    private async Task ClearOpenCacheAsync()
    {
        await _cacheService.DeleteCacheAsync(OpenProjectCacheKey);
        await _cacheService.DeleteCacheAsync(OpenFolderCacheKey);
    }

    private async Task RestoreOpenProject()
    {
        // Attempt to restore open project
        var projectPath = await _cacheService.RetrieveCacheAsync(OpenProjectCacheKey);
        if (projectPath is not null)
        {
            // Restore project and return
            await OpenProjectAsync(projectPath);
            return;
        }

        // Attempt to restore open folder
        var folderPath = await _cacheService.RetrieveCacheAsync(OpenFolderCacheKey);
        if (folderPath is not null)
        {
            // Restore folder and return
            await OpenFolderAsync(folderPath);
            return;
        }
    }

}
