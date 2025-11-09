// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Mipser.Bindables.Files.Abstract;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mipser.Bindables.Files;

/// <summary>
/// A folder in the explorer.
/// </summary>
public class BindableFolder : BindableFilesItemBase
{
    private readonly IFolder _folder;
    private ObservableCollection<BindableFilesItemBase> _items;
    private bool _childrenNotCalculated;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFolder"/> class.
    /// </summary>
    public BindableFolder(FileService fileService, IFolder folder) : base(fileService)
    {
        _folder = folder;

        _items = [];
        ChildrenNotLoaded = true;
    }

    /// <summary>
    /// Gets the folder's children.
    /// </summary>
    public ObservableCollection<BindableFilesItemBase> Children
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    /// <summary>
    /// Gets a value indicating whether or not the children have been loaded.
    /// </summary>
    public bool ChildrenNotLoaded
    {
        get => _childrenNotCalculated;
        set => SetProperty(ref _childrenNotCalculated, value);
    }

    /// <inheritdoc/>
    protected override IFilesItem? Item => _folder;

    /// <summary>
    /// Creates a new file in the folder.
    /// </summary>
    /// <param name="name">The name of the file.</param>
    /// <returns>The file created</returns>
    public async Task<BindableFile?> CreateFileAsync(string name)
    {
        // Can't create a file in a non-existent folder
        if (Path is null)
            return null;

        // Create file
        var path = System.IO.Path.Combine(Path, name);
        var file = await FileService.CreateFileAsync(path);

        // Failed
        if (file is null)
            return null;

        // Track child if children are tracked
        if (!ChildrenNotLoaded && !Children.Contains(file))
            Children.Add(file);

        return file;
    }

    /// <summary>
    /// Creates a new file in the folder.
    /// </summary>
    /// <param name="name">The name of the folder.</param>
    /// <returns>The file created</returns>
    public async Task<BindableFolder?> CreateFolderAsync(string name)
    {
        // Can't create a folder in a non-existent folder
        if (Path is null)
            return null;

        // Create folder
        var path = System.IO.Path.Combine(Path, name);
        var folder = await FileService.CreateFolderAsync(path);

        // Failed
        if (folder is null)
            return null;

        // Track child if children are tracked
        if (!ChildrenNotLoaded && !Children.Contains(folder))
            Children.Add(folder);

        return folder;
    }

    /// <summary>
    /// Opens a child folder.
    /// </summary>
    /// <param name="name">The name of the folder.</param>
    public async Task<BindableFolder?> OpenFolderAsync(string name)
    {
        // Can't create a folder in a non-existent folder
        if (Path is null)
            return null;

        // Open folder
        var path = System.IO.Path.Combine(Path, name);
        var folder = await FileService.GetFolderAsync(path);

        // Failed
        if (folder is null)
            return null;

        // Track child if children are tracked
        if (!ChildrenNotLoaded && !Children.Contains(folder))
            Children.Add(folder);

        return folder;
    }

    /// <summary>
    /// Loads the node's children.
    /// </summary>
    public async Task LoadChildrenAsync(bool recursive = false)
    {
        var items = await _folder.GetItemsAsync();
        var children = items.Select(x =>
        {
            return x switch
            {
                IFile file => FileService.GetOrAddTrackedFile(file),
                IFolder folder => FileService.GetOrAddTrackedFolder(folder),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<BindableFilesItemBase>(),
            };
        });

        ChildrenNotLoaded = false;

        Children.Clear();
        foreach (var item in children)
        {
            Children.Add(item);

            // Recursively load children if recursing
            if (recursive && item is BindableFolder folder)
                await folder.LoadChildrenAsync(recursive);
        }
    }
}
