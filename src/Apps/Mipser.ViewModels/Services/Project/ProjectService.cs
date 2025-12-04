// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Models.ProjectConfig;
using Mipser.Services.Build;
using Mipser.Services.Files;
using Mipser.Services.Settings;
using Mipser.Windows.Services.Cache;
using System.Threading.Tasks;

namespace Mipser.Services.Project;

/// <summary>
/// An implementation of the <see cref="IProjectService"/> interface.
/// </summary>
public class ProjectService : IProjectService
{
    private const string OpenProjectCacheKey = "OpenProject";
    private const string OpenFolderCacheKey = "OpenFolder";

    private readonly IMessenger _messenger;
    private readonly ICacheService _cacheService;
    private readonly IFileService _fileService;

    private BindableFolder? _objFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectService"/> class.
    /// </summary>
    public ProjectService(IMessenger messenger, ISettingsService settingsService, ICacheService cacheService, IFileService fileService)
    {
        _messenger = messenger;
        _cacheService = cacheService;
        _fileService = fileService;

        if (settingsService.Local.GetValue<bool>(SettingsKeys.RestoreOpenProject))
        {
            _ = RestoreOpenProject();
        }
    }

    /// <inheritdoc/>
    public ProjectConfig? Config { get; private set; }

    /// <inheritdoc/>
    public BindableFolder? ProjectRootFolder { get; private set; }

    /// <inheritdoc/>
    public void OpenFolder(BindableFolder folder, bool cacheState = true)
    {
        // Clear the cached obj folder
        _objFolder = null;

        // Change the root folder
        ProjectRootFolder = folder;

        // Send a message notifying the change
        _messenger.Send(new FolderOpenedMessage(folder));

        // Update the state.
        if (cacheState)
        {
            _ = CacheOpenProject(true);
        }
    }

    /// <inheritdoc/>
    public async Task OpenFolderAsync(string path, bool cacheState = true)
    {
        // Load the folder
        var folder = await _fileService.GetFolderAsync(path);
        if (folder is null)
            return;

        // Open the folder
        OpenFolder(folder, cacheState);
    }

    /// <inheritdoc/>
    public async Task OpenProjectAsync(ProjectConfig config, bool cacheState = true)
    {
        Config = config;
        if (config.RootFolderPath is null)
            return;

        await OpenFolderAsync(config.RootFolderPath, false);

        if (cacheState)
        {
            await CacheOpenProject();
        }
    }

    /// <inheritdoc/>
    public async Task OpenProjectAsync(string path, bool cacheState = true)
    {
        // Attempt to load the file
        var file = await _fileService.GetFileAsync(path);
        if (file is null)
            return;

        // Attempt to open the file as a stream
        var stream = await file.GetReadStreamAsync();
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

    private async Task CacheOpenProject(bool folder = false)
    {
        // Clear current cache
        await _cacheService.DeleteCacheAsync(OpenProjectCacheKey);
        await _cacheService.DeleteCacheAsync(OpenFolderCacheKey);

        // Cache value and key
        var (key, value) = folder switch
        {
            false => (OpenProjectCacheKey, Config?.ConfigPath),
            true => (OpenFolderCacheKey, ProjectRootFolder?.Path),
        };
        await _cacheService.CacheAsync(key, value);
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
