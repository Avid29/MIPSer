// Adam Dernis 2024

using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using Mipser.Windows.Services.FileSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Mipser.Windows.Services.FileSystem;

/// <summary>
/// An <see cref="IFileSystemService"/> implementation wrapping <see cref="StorageFile"/>.
/// </summary>
public class FileSystemService : IFileSystemService
{
    /// <inheritdoc/>
    public async Task<IFile?> CreateFileAsync(string path)
    {
        try
        {
            // Split the path
            var folderPath = System.IO.Path.GetDirectoryName(path);
            var fileName = System.IO.Path.GetFileName(path);

            // Create the file in the parent folder.
            var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            var file = await folder.CreateFileAsync(fileName);
            return new File(file);
        }
        catch
        {
            return null;
        }
    }
    
    /// <inheritdoc/>
    public async Task<IFolder?> CreateFolderAsync(string path)
    {
        try
        {
            // Split the path
            var folderPath = System.IO.Path.GetDirectoryName(path);
            var fileName = System.IO.Path.GetFileName(path);

            // Create the file in the parent folder.
            var parent = await StorageFolder.GetFolderFromPathAsync(folderPath);
            var folder = await parent.CreateFolderAsync(fileName);
            return new Folder(folder);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IFile?> GetFileAsync(string path)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            return new File(file);
        } catch
        {
            return null;
        }
    }
    
    /// <inheritdoc/>
    public async Task<IFolder?> GetFolderAsync(string path)
    {
        try
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(path);
            return new Folder(folder);
        } catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IFile?> PickFileAsync(params string[] types)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.List
        };

        if (types.Length is 0)
            types = ["*"];

        foreach (var type in types)
        {
            picker.FileTypeFilter.Add(type);
        }

        InitWindow(picker);
        StorageFile file = await picker.PickSingleFileAsync();

        if (file is null)
            return null;

        return new File(file);
    }

    /// <inheritdoc/>
    public async Task<IFolder?> PickFolderAsync()
    {
        var picker = new FolderPicker
        {
            ViewMode = PickerViewMode.List,
            FileTypeFilter = { "*" },
        };

        InitWindow(picker);
        var storageFolder = await picker.PickSingleFolderAsync();

        if (storageFolder is null)
            return null;

        return new Folder(storageFolder);
    }

    /// <inheritdoc/>
    public async Task<IFile?> PickSaveFileAsync(string filename)
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.Unspecified,
            DefaultFileExtension = ".asm",
            SuggestedFileName = filename,
        };

        InitWindow(picker);
        var storageFile = await picker.PickSaveFileAsync();

        if (storageFile is null)
            return null;

        return new File(storageFile);
    }

    private void InitWindow(object picker)
    {
        nint windowHandle = WindowNative.GetWindowHandle(App.Current.Window);
        InitializeWithWindow.Initialize(picker, windowHandle);
    }
}
