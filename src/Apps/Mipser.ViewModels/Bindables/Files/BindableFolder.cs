// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Mipser.Bindables.Files.Abstract;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using System.Collections.ObjectModel;
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
        ChildrenNotCalculated = true;
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
    /// Gets a value indicating whether or not the children have been calculated.
    /// </summary>
    public bool ChildrenNotCalculated
    {
        get => _childrenNotCalculated;
        set => SetProperty(ref _childrenNotCalculated, value);
    }

    /// <inheritdoc/>
    protected override IFilesItem? Item => _folder;

    /// <summary>
    /// Loads the node's children.
    /// </summary>
    public async Task LoadChildrenAsync()
    {
        var items = await _folder.GetItemsAsync();
        var children = items.Select(x =>
        {
            return x switch
            {
                IFile file => FileService.GetOrAddTrackedFile(file),
                IFolder folder => FileService.GetFolder(folder),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<BindableFilesItemBase>(),
            };
        });

        ChildrenNotCalculated = false;

        Children.Clear();
        foreach (var item in children)
        {
            Children.Add(item);
        }
    }
}
