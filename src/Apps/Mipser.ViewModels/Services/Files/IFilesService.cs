// Adam Dernis 2023

using Mipser.Services.Files.Models;
using System.Threading.Tasks;

namespace Mipser.Services.Files;

/// <summary>
/// An interface for a files service
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Attempts to get a file.
    /// </summary>
    /// <param name="path">The path of the file</param>
    Task<IFile?> TryGetFileAsync(string path);

    /// <summary>
    /// Attempts to pick a file to open.
    /// </summary>
    /// <returns>An <see cref="IFile"/> to open, if successful.</returns>
    Task<IFile?> TryPickAndOpenFileAsync();


    /// <summary>
    /// Attempts to pick a folder to open.
    /// </summary>
    /// <returns>An <see cref="IFolder"/> to open, if successful.</returns>
    Task<IFolder?> TryPickAndOpenFolderAsync();

    /// <summary>
    /// Attempts to pick a file to open.
    /// </summary>
    /// <returns>An <see cref="IFile"/> to save to, if successful.</returns>
    Task<IFile?> TryPickAndSaveFileAsync(string filename);
}
