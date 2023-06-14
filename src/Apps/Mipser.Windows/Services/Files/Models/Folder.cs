// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using Mipser.Services.Files.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Mipser.Windows.Services.Files.Models;

/// <summary>
/// An <see cref="IFolder"/> implementation wrapping a <see cref="StorageFolder"/>.
/// </summary>
public class Folder : IFolder
{
    private readonly StorageFolder _storageFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="Folder"/> class.
    /// </summary>
    /// <param name="storageFolder"></param>
    public Folder(StorageFolder storageFolder)
    {
        _storageFolder = storageFolder;
    }

    /// <inheritdoc/>
    public string Name => _storageFolder.Name;
    
    /// <inheritdoc/>
    public string Path => _storageFolder.Path;
    
    /// <inheritdoc/>
    public async Task<IFilesItem[]> GetItemsAsync()
    {
        var items = await _storageFolder.GetItemsAsync();
        return (IFilesItem[])items.Select(x =>
        {
            return x switch
            {
                StorageFile file => new File(file),
                StorageFolder folder => new Folder(folder),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<IFilesItem>(),
            };
        });

        //var folders = await GetFoldersAsync();
        //var files = await GetFilesAsync();

        //return folders.Select(x => (IFilesItem)x).Concat(files.Select(x => (IFilesItem)x)).ToArray();
    }
    
    /// <inheritdoc/>
    public async Task<IFolder[]> GetFoldersAsync()
    {
        var folders = await _storageFolder.GetFoldersAsync();
        return folders.Select(x => (IFolder)new Folder(x)).ToArray();
    }
    
    /// <inheritdoc/>
    public async Task<IFile[]> GetFilesAsync()
    {
        var files = await _storageFolder.GetFilesAsync();
        return files.Select(x => (IFile)new File(x)).ToArray();
    }
}
