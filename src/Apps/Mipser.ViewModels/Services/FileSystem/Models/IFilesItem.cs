// Adam Dernis 2024

namespace Mipser.Services.FileSystem.Models;

/// <summary>
/// An interface for a files item.
/// </summary>
public interface IFilesItem
{
    /// <summary>
    /// Gets the name of the files item.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the path of the files item.
    /// </summary>
    string Path { get; }
}
