// Adam Dernis 2024

using Mipser.Services.FileSystem;
using Mipser.Services.FileSystem.Models;
using Mipser.Windows.Services.FileSystem.Models;
using System;
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
    public async Task<IFile?> TryGetFileAsync(string path) => new File(await StorageFile.GetFileFromPathAsync(path));

    /// <inheritdoc/>
    public async Task<IFile?> TryPickAndOpenFileAsync()
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.List,
            FileTypeFilter = { "*" },
        };

        InitWindow(picker);
        StorageFile file = await picker.PickSingleFileAsync();

        if (file is null)
            return null;

        return new File(file);
    }

    /// <inheritdoc/>
    public async Task<IFolder?> TryPickAndOpenFolderAsync()
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
    public async Task<IFile?> TryPickAndSaveFileAsync(string filename)
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
