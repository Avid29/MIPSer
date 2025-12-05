// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using Mipser.Bindables.Files;
using Mipser.Services.Files.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    private readonly Dictionary<string, BindableFileItem> _openItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    public FileService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
        
        _openItems = [];
    }

    /// <inheritdoc/>
    public async Task<BindableFolder?> GetFolderAsync(string path)
    {
        // Check if the folder is already tracked, 
        // and retrieve it if so.
        if (TryGetItem(path, out BindableFolder? value))
            return value;

        var folder = await _fileSystemService.GetFolderAsync(path);
        if (folder is null)
            return null;

        return TrackFolder(folder);
    }

    /// <inheritdoc/>
    public async Task<BindableFile?> GetFileAsync(string path)
    {
        // Check if the file is already tracked, 
        // and retrieve it if so.
        if (TryGetItem(path, out BindableFile? value))
            return value;

        // Get basic file
        var file = await _fileSystemService.GetFileAsync(path);
        if (file is null)
            return null;

        // Create and track new bindable
        return TrackFile(file);
    }

    /// <inheritdoc/>
    public async Task<BindableFileItem?> GetFileItemAsync(string path)
    {
        return IsFile(path) switch
        {
            true => await GetFileAsync(path),
            false => await GetFolderAsync(path),
        };
    }
    
    /// <inheritdoc/>
    public async Task<BindableFolder?> PickFolderAsync()
    {
        var folder = await _fileSystemService.PickFolderAsync();
        if (folder is null)
            return null;

        return TrackFolder(folder);
    }

    /// <inheritdoc/>
    public async Task<BindableFile?> PickFileAsync(params string[] types)
    {
        var file = await _fileSystemService.PickFileAsync(types);
        if (file is null)
            return null;

        return TrackFile(file);
    }

    internal BindableFolder TrackFolder(IFolder folder)
    {
        // Check if the folder is already tracked, 
        // and retrieve it if so.
        var key = folder.Path;
        if (TryGetItem(key, out BindableFolder? value))
            return value;

        // Create and track new bindable
        var bindable = new BindableFolder(this, folder);
        _openItems.Add(key, bindable);
        return bindable;
    }

    internal BindableFile TrackFile(IFile file)
    {
        // Check if the file is already tracked, 
        // and retrieve it if so.
        var key = file.Path;
        if (TryGetItem(key, out BindableFile? value))
            return value;

        // Create and track new bindable
        var bindable = new BindableFile(this, file);
        _openItems.Add(key, bindable);
        return bindable;
    }

    internal BindableFileItem TrackFileItem(IFileItem item)
    {
        return item switch
        {
            IFolder folder => TrackFolder(folder),
            IFile file => TrackFileItem(file),
            _ => ThrowHelper.ThrowArgumentException<BindableFileItem>(nameof(item)),
        };
    }

    internal void UntrackFileItem(BindableFileItem item)
    {
        var key = item.Path;
        if (key is null || !_openItems.Remove(key))
            return;
    }

    internal async Task RenameTrackedItemAsync(string oldPath, string newPath)
    {
        var item = await GetFileItemAsync(oldPath);
        if (item is null)
            return;

        // Untrack item as-is
        UntrackFileItem(item);

        switch (item)
        {
            // Get new folder item child
            case BindableFolder folder:
                var childFolder = await _fileSystemService.GetFolderAsync(newPath);
                if (childFolder is null)
                    return;

                folder.FileItem = childFolder;
                break;
                
            // Get new file item child
            case BindableFile file:
                var childFile = await _fileSystemService.GetFileAsync(newPath);
                if (childFile is null)
                    return;

                file.FileItem = childFile;
                break;
        }

        // Retrack
        _openItems.Add(newPath, item);
    }

    private bool TryGetItem<T>(string path, [NotNullWhen(true)] out T? item)
        where T : BindableFileItem
    {
        item = null;

        if(!_openItems.TryGetValue(path, out var value))
            return false;

        if (value is not T result)
            return false;

        item = result;
        return true;
    }

    private static bool IsFile(string? path) => path is null || Path.GetFileName(path) is not null;
}
