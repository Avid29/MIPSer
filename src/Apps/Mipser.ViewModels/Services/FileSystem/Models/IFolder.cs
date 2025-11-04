// Adam Dernis 2024

using System.Threading.Tasks;

namespace Mipser.Services.FileSystem.Models;

/// <summary>
/// An interface for a folder.
/// </summary>
public interface IFolder : IFilesItem
{
    /// <summary>
    /// Gets the files and subfolders in this folder.
    /// </summary>
    /// <returns></returns>
    Task<IFilesItem[]> GetItemsAsync();

    /// <summary>
    /// Gets the subfolders in this folder.
    /// </summary>
    /// <returns></returns>
    Task<IFolder[]> GetFoldersAsync();

    /// <summary>
    /// Gets the files in this folder.
    /// </summary>
    /// <returns></returns>
    Task<IFile[]> GetFilesAsync();
}
