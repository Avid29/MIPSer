// Adam Dernis 2024

using System.Threading.Tasks;

namespace Mipser.Services.Files.Models;

/// <summary>
/// An interface for a files item.
/// </summary>
public interface IFileItem
{
    /// <summary>
    /// Gets the name of the files item.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the path of the files item.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Delete the file item.
    /// </summary>
    Task DeleteAsync();
}
