// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Mipser.Services;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mipser.Bindables.Files;

/// <summary>
/// A folder in the explorer.
/// </summary>
public partial class BindableFolder : BindableFileItem<IFolder>
{
    private readonly Dictionary<BindableFileItem, BindableFile> _virtualParents;
    private FileSystemWatcher? _watcher;
    private bool _childrenNotCalculated;


    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFolder"/> class.
    /// </summary>
    public BindableFolder(FileService fileService, IFolder folder) : base(fileService)
    {
        _virtualParents = [];

        FileItem = folder;

        Children = [];
        ChildrenNotLoaded = true;

        OpenInExplorerCommand = new(OpenInExplorer);
        OpenInWindowsTerminalCommand = new(OpenInWindowsTerminal);
    }

    /// <summary>
    /// Gets the folder's children.
    /// </summary>
    public override ObservableCollection<BindableFileItem> Children { get; }

    /// <summary>
    /// Gets a value indicating whether or not the children have been loaded.
    /// </summary>
    public bool ChildrenNotLoaded
    {
        get => _childrenNotCalculated;
        set => SetProperty(ref _childrenNotCalculated, value);
    }

    /// <inheritdoc/>
    protected internal override IFolder FileItem
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Path));
            }
        }
    }

    /// <summary>
    /// Loads the node's children.
    /// </summary>
    public async Task LoadChildrenAsync(bool recursive = false)
    {
        Guard.IsNotNull(FileItem);

        var items = await FileItem.GetItemsAsync();
        var children = items.Select(x =>
        {
            return x switch
            {
                IFile file => FileService.TrackFile(file),
                IFolder folder => FileService.TrackFolder(folder),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<BindableFileItem>(),
            };
        });

        ChildrenNotLoaded = false;

        Children.Clear();
        SetupWatcher();

        foreach (var item in children.OrderBy(x => x.Name.EndsWith(".obj")))
        {
            TrackChild(item);

            // Recursively load children if recursing
            if (recursive && item is BindableFolder folder)
                await folder.LoadChildrenAsync(recursive);
        }
    }

    internal void TrackChild(BindableFileItem item)
    {
        var nameAsAsm = $"{System.IO.Path.GetFileNameWithoutExtension(item.Name)}.asm";
        var parentAsm = Children.OfType<BindableFile>().FirstOrDefault(x => x.Name == nameAsAsm);
        if (parentAsm is not null)
        {
            parentAsm.TrackAsChild(item);
            _virtualParents.Add(item, parentAsm);
            return;
        }

        Children.Add(item);
    }

    internal void UntrackChild(BindableFileItem item)
    {
        if (item is BindableFile file && _virtualParents.ContainsKey(file))
        {
            _virtualParents[file].UntrackChild(item);
            _virtualParents.Remove(file);
        }
        else
        {
            Children.Remove(item);
        }
    }

    private void SetupWatcher()
    {
        Guard.IsNotNull(Path);

        _watcher = new(Path);
        _watcher.Created += OnChildFileItemCreated;
        _watcher.Deleted += OnChildFileItemDeleted;
        _watcher.Renamed += OnChildFileItemRenamed;
        // TODO: Handle other events

        _watcher.EnableRaisingEvents = true;
    }

    private async void OnChildFileItemCreated(object sender, FileSystemEventArgs e)
    {
        // Retrieve/track the item
        var child = await FileService.GetFileItemAsync(e.FullPath);
        if (child is null)
            return;

        // Track as child
        Service.Get<IDispatcherService>().RunOnUIThread(() =>
        {
            TrackChild(child);
        });
    }

    private async void OnChildFileItemDeleted(object sender, FileSystemEventArgs e)
    {
        // Retrieve the item
        var child = await FileService.GetFileItemAsync(e.FullPath);
        if (child is null)
            return;

        // Untrack the item
        Service.Get<IDispatcherService>().RunOnUIThread(() =>
        {
            FileService.UntrackFileItem(child);
            UntrackChild(child);
        });
    }

    private async void OnChildFileItemRenamed(object sender, RenamedEventArgs e)
    {
        Service.Get<IDispatcherService>().RunOnUIThread(async () =>
        {
            await FileService.RenameTrackedItemAsync(e.OldFullPath, e.FullPath);
        });
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        if (_watcher is null)
            return;

        _watcher.Created -= OnChildFileItemCreated;
        _watcher.Deleted -= OnChildFileItemDeleted;
        _watcher.Renamed -= OnChildFileItemRenamed;
    }
}
