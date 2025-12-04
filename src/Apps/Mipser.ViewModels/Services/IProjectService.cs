// Avishai Dernis 2025

using Mipser.Models.ProjectConfig;
using Mipser.Services.Files.Models;
using System.Threading.Tasks;

namespace Mipser.Services;

/// <summary>
/// An interface for a service that manages project layout.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Gets the open project.
    /// </summary>
    public Project? Project { get; }

    /// <summary>
    /// Gets the root folder of the project.
    /// </summary>
    public IFolder? ProjectRootFolder { get; }

    /// <summary>
    /// Opens a folder as the new project folder.
    /// </summary>
    public void OpenFolder(IFolder folder, bool cacheState = true);

    /// <summary>
    /// Opens a folder as the new project folder by path.
    /// </summary>
    public Task OpenFolderAsync(string path, bool cacheState = true);

    /// <summary>
    /// Opens a project by config.
    /// </summary>
    public Task OpenProjectAsync(ProjectConfig config, bool cacheState = true);

    /// <summary>
    /// Opens a project by config path.
    /// </summary>
    public Task OpenProjectAsync(string filePath, bool cacheState = true);
}
