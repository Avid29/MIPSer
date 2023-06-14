// Adam Dernis 2023

using System.IO;
using System.Threading.Tasks;

namespace Mipser.Services.Files.Models;

/// <summary>
/// An interface for a file.
/// </summary>
public interface IFile
{
    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the path of the file.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Opens a <see cref="Stream"/> for reading from the file.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> returning the requested <see cref="FileStream"/>.</returns>
    Task<Stream> OpenStreamForReadAsync();

    /// <summary>
    /// Opens a <see cref="Stream"/> for writing to the file.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> returning the requested <see cref="FileStream"/>.</returns>
    Task<Stream> OpenStreamForWriteAsync();

    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the file is deleted.</returns>
    Task DeleteAsync();
}
