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
public class FileService
{
    // TODO: Untracking out of use files

    private IFileSystemService _fileSystemService;
    private Dictionary<string, BindableFile> _openFiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    public FileService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;

        _openFiles = [];
    }

    /// <summary>
    /// Gets a file from a path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    public async Task<BindableFile?> GetFileAsync(string path)
    {
        // Get storage file
        var file = await _fileSystemService.GetFileAsync(path);
        if (file is null)
            return null;

        return GetOrAddTrackedFile(file);
    }

    /// <summary>
    /// Opens a file picker to select an <see cref="IFile"/>.
    /// </summary>
    /// <returns>The selected <see cref="IFile"/>.</returns>
    public async Task<BindableFile?> PickFileAsyc()
    {
        var file = await _fileSystemService.PickFileAsync();
        if (file is null)
            return null;

        return GetOrAddTrackedFile(file);
    }

    private BindableFile GetOrAddTrackedFile(IFile file)
    {
        // Check if the file is already tracked, 
        // and retrieve it if so.
        var key = file.Path;
        if (_openFiles.ContainsKey(key))
            return _openFiles[key];

        // Create and track new bindable
        var bindable = new BindableFile(file);
        _openFiles.Add(key, bindable);
        return bindable;
    }
}
