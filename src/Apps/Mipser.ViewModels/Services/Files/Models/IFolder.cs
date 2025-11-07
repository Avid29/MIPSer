// Adam Dernis 2024

using System.Threading.Tasks;

namespace Mipser.Services.Files.Models;

/// <summary>
/// An interface for a folder.
/// </summary>
public interface IFolder : IFilesItem
{
    /// <summary>
    /// Gets the files and subfolders in this folder.
    /// </summary>
    Task<IFilesItem[]> GetItemsAsync();

    /// <summary>
    /// Gets the subfolders in this folder.
    /// </summary>
    Task<IFolder[]> GetFoldersAsync();

    /// <summary>
    /// Gets the files in this folder.
    /// </summary>
    Task<IFile[]> GetFilesAsync();
}
