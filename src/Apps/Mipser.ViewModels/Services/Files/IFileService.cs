// Avishai Dernis 2025

using Mipser.Bindables.Files;
using System.Threading.Tasks;

namespace Mipser.Services.Files;

/// <summary>
/// An interface for a service to manager files in use.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Gets an anonymous file.
    /// </summary>
    public BindableFile GetAnonymousFile();

    /// <summary>
    /// Creates a file.
    /// </summary>
    /// <param name="path">The name of the file.</param>
    public Task<BindableFile?> CreateFileAsync(string path);

    /// <summary>
    /// Creates a folder.
    /// </summary>
    /// <param name="path">The name of the folder.</param>
    public Task<BindableFolder?> CreateFolderAsync(string path);

    /// <summary>
    /// Gets a file from a path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    public Task<BindableFile?> GetFileAsync(string path);

    /// <summary>
    /// Gets a folder from a path.
    /// </summary>
    /// <param name="path">The path of the folder.</param>
    public Task<BindableFolder?> GetFolderAsync(string path);

    /// <summary>
    /// Opens a file picker to select an <see cref="BindableFile"/>.
    /// </summary>
    /// <returns>The selected <see cref="BindableFile"/>.</returns>
    public Task<BindableFile?> PickFileAsync(params string[] types);

    /// <summary>
    /// Opens a file picker to select an <see cref="BindableFolder"/>.
    /// </summary>
    /// <returns>The selected <see cref="BindableFolder"/>.</returns>
    public Task<BindableFolder?> PickFolderAsync();
}
