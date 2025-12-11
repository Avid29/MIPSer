// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Models;
using Mipser.Models.Files;
using Mipser.Models.ProjectConfig;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using Mipser.Services.Settings;
using System.IO;
using System.Threading.Tasks;

namespace Mipser.Services;

/// <summary>
/// An implementation of the <see cref="IProjectService"/> interface.
/// </summary>
public class ProjectService : IProjectService
{
    private const string RecentProjectsCacheKey = "RecentProjects";
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
    public SourceFile? GetSourceFile(string filePath)
    {
        if (Project is null)
            return null;

        return Project.SourceFiles[filePath];
    }

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
    public async Task OpenPathAsyc(string path, bool cacheState = true)
    {
        if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
        {
            await OpenFolderAsync(path);
        }
        else
        {
            await OpenProjectAsync(path);
        }
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
        await ClearOpenCacheAsync();

        // Get current cache
        var recent = await _cacheService.RetrieveCacheAsync<RecentFileItemsCache>(RecentProjectsCacheKey);
        if (recent is null)
            recent = new();

        // Append proper path
        var path = folder switch
        {
            false => Project?.Config?.ConfigPath,
            true => ProjectRootFolder?.Path,
        };
        recent.Append(path, 10);

        // Cache updated cache model
        await _cacheService.CacheAsync(RecentProjectsCacheKey, recent);
    }

    private async Task ClearOpenCacheAsync()
    {
        await _cacheService.DeleteCacheAsync(OpenProjectCacheKey);
        await _cacheService.DeleteCacheAsync(OpenFolderCacheKey);
    }

    private async Task RestoreOpenProject()
    {
        // Attempt to retrieve the current cache
        var recent = await _cacheService.RetrieveCacheAsync<RecentFileItemsCache>(RecentProjectsCacheKey);
        if (recent is null)
            return;

        // Get the path of the most recent item
        var path = recent.Paths.First?.Value;
        if (path is null)
            return;

        await OpenPathAsyc(path);
    }
}
