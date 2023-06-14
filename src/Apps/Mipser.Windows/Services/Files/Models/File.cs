// Adam Dernis 2023

using Mipser.Services.Files.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Mipser.Windows.Services.Files.Models;

/// <summary>
/// A <see cref="IFile"/> implementation wrapping <see cref="StorageFile"/>.
/// </summary>
public sealed class File : IFile
{
    private readonly StorageFile _storageFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="File"/> class.
    /// </summary>
    public File(StorageFile storageFile)
    {
        _storageFile = storageFile;
    }
    
    /// <inheritdoc/>
    public string Name => _storageFile.Name;
    
    /// <inheritdoc/>
    public string Path => _storageFile.Path;

    /// <inheritdoc/>
    public Task<Stream> OpenStreamForReadAsync() => _storageFile.OpenStreamForReadAsync();

    /// <inheritdoc/>
    public Task<Stream> OpenStreamForWriteAsync() => _storageFile.OpenStreamForWriteAsync();
    
    /// <inheritdoc/>
    public async Task DeleteAsync() => await _storageFile.DeleteAsync();
}
