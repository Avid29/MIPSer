// Avishai Dernis 2025

using Mipser.Bindables.Files;
using Mipser.Services.Files.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mipser.Services.Files;

/// <summary>
/// A wrapper for the <see cref="IFileSystemService"/> which also tracks open files.
/// </summary>
public class FileService : IFileService
{
    // TODO: Untracking out of use files

    private readonly IFileSystemService _fileSystemService;
    private readonly Dictionary<string, BindableFile> _openFiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    public FileService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;

        _openFiles = [];
    }
    
    /// <inheritdoc/>
    public BindableFile GetAnonymousFile()
    {
        return new BindableFile(this);
    }
    
    /// <inheritdoc/>
    public async Task<BindableFile?> GetFileAsync(string path)
    {
        // Get storage file
        var file = await _fileSystemService.GetFileAsync(path);
        if (file is null)
            return null;

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
    public async Task<BindableFile?> PickFileAsyc()
    {
        var file = await _fileSystemService.PickFileAsync();
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
        // TODO: Track folders
        return new BindableFolder(this, folder);
    }

    internal BindableFile GetOrAddTrackedFile(IFile file)
    {
        // Check if the file is already tracked, 
        // and retrieve it if so.
        var key = file.Path;
        if (_openFiles.ContainsKey(key))
            return _openFiles[key];

        // Create and track new bindable
        var bindable = new BindableFile(this, file);
        _openFiles.Add(key, bindable);
        return bindable;
    }
}
