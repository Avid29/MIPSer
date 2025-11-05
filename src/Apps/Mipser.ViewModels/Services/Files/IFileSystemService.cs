// Adam Dernis 2024

using Mipser.Services.Files.Models;
using System.Threading.Tasks;

namespace Mipser.Services.Files;

/// <summary>
/// An interface for a files service
/// </summary>
public interface IFileSystemService
{
    /// <summary>
    /// Attempts to get a file.
    /// </summary>
    /// <param name="path">The path of the file</param>
    Task<IFile?> GetFileAsync(string path);

    /// <summary>
    /// Attempts to pick a folder to open.
    /// </summary>
    /// <returns>An <see cref="IFolder"/> to open, if successful.</returns>
    Task<IFolder?> PickFolderAsync();

    /// <summary>
    /// Attempts to pick a file to open.
    /// </summary>
    /// <returns>An <see cref="IFile"/> to open, if successful.</returns>
    Task<IFile?> PickFileAsync();

    /// <summary>
    /// Attempts to pick a file to open.
    /// </summary>
    /// <returns>An <see cref="IFile"/> to save to, if successful.</returns>
    Task<IFile?> PickSaveFileAsync(string filename);
}
