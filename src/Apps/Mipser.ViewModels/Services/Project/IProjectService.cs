// Avishai Dernis 2025

using Mipser.Bindables.Files;
using System.Threading.Tasks;

namespace Mipser.Services.Project;

/// <summary>
/// An interface for a service that manages project layout.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Gets the root folder of the project.
    /// </summary>
    public BindableFolder? ProjectRootFolder { get; }

    /// <summary>
    /// Opens a folder as the new project folder.
    /// </summary>
    public void OpenFolder(BindableFolder folder);

    /// <summary>
    /// Gets the object folder.
    /// </summary>
    public Task<BindableFolder?> GetObjectFolderAsync();
}
