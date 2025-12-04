// Avishai Dernis 2025

using Mipser.Bindables.Files;
using Mipser.Bindables.Files.Abstract;
using Mipser.Services.Files.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Mipser.Services.Files;

/// <summary>
/// A wrapper for the <see cref="IFileSystemService"/> which also tracks open files.
/// </summary>
public class FileService : IFileService
{
    // TODO: Untracking out of use files

    private readonly IFileSystemService _fileSystemService;
    private readonly Dictionary<string, BindableFolder> _openFolders;
    private readonly Dictionary<string, BindableFile> _openFiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    public FileService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;

        _openFolders = [];
        _openFiles = [];
    }
    
    /// <inheritdoc/>
    public BindableFile GetAnonymousFile() => new(this);

    /// <inheritdoc/>
    public async Task<BindableFile?> GetFileAsync(string path)
    {
        // Get basic file
        var file = await _fileSystemService.GetFileAsync(path);
        if (file is null)
            return null;
        
        // Track and return
        return GetOrAddTrackedFile(file);
    }
    
    /// <inheritdoc/>
    public async Task<BindableFolder?> GetFolderAsync(string path)
    {
        var folder = await _fileSystemService.GetFolderAsync(path);
        if (folder is null)
            return null;

        return GetOrAddTrackedFolder(folder);
    }

    /// <inheritdoc/>
    public async Task<BindableFileItem?> GetFileItemAsync(string path)
    {
        return (Path.GetFileName(path) is null) switch
        {
            true => await GetFolderAsync(path),
            false => await GetFileAsync(path),
        };
    }

    /// <inheritdoc/>
    public async Task<BindableFile?> PickFileAsync(params string[] types)
    {
        var file = await _fileSystemService.PickFileAsync(types);
        if (file is null)
            return null;

        return GetOrAddTrackedFile(file);
    }
    
    /// <inheritdoc/>
    public async Task<BindableFolder?> PickFolderAsync()
    {
        var folder = await _fileSystemService.PickFolderAsync();
        if (folder is null)
            return null;

        return GetOrAddTrackedFolder(folder);
    }

    internal BindableFolder GetOrAddTrackedFolder(IFolder folder)
    {
        // Check if the file is already tracked, 
        // and retrieve it if so.
        var key = folder.Path;
        if (_openFolders.TryGetValue(key, out BindableFolder? value))
            return value;

        // Create and track new bindable
        var bindable = new BindableFolder(this, folder);
        _openFolders.Add(key, bindable);
        return bindable;
    }

    internal BindableFile GetOrAddTrackedFile(IFile file)
    {
        // Check if the file is already tracked, 
        // and retrieve it if so.
        var key = file.Path;
        if (_openFiles.TryGetValue(key, out BindableFile? value))
            return value;

        // Create and track new bindable
        var bindable = new BindableFile(this, file);
        _openFiles.Add(key, bindable);
        return bindable;
    }
}
