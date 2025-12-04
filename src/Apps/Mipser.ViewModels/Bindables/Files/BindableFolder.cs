// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using Mipser.Bindables.Files.Abstract;
using Mipser.Services;
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
public class BindableFolder : BindableFileItemBase
{
    private readonly IFolder _folder;
    private FileSystemWatcher? _watcher;
    private bool _childrenNotCalculated;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFolder"/> class.
    /// </summary>
    public BindableFolder(FileService fileService, IFolder folder) : base(fileService)
    {
        _folder = folder;

        Children = [];
        ChildrenNotLoaded = true;
    }

    /// <summary>
    /// Gets the folder's children.
    /// </summary>
    public override ObservableCollection<BindableFileItemBase> Children { get; }

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
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<BindableFileItemBase>(),
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

    internal void TrackChild(BindableFileItemBase item)
    {
        var nameAsAsm = $"{System.IO.Path.GetFileNameWithoutExtension(item.Name)}.asm";
        var parentAsm = Children.OfType<BindableFile>().FirstOrDefault(x => x.Name == nameAsAsm);
        if (parentAsm is not null)
        {
            parentAsm.TrackAsChild(item);
            return;
        }

        Children.Add(item);
    }

    private void SetupWatcher()
    {
        Guard.IsNotNull(Path);

        _watcher = new(Path);
        _watcher.Created += OnChildFileItemCreated;
        // TODO: Handle other events

        _watcher.EnableRaisingEvents = true;
    }

    private async void OnChildFileItemCreated(object sender, FileSystemEventArgs e)
    {
        var child = await FileService.GetFileItemAsync(e.FullPath);
        if (child is null)
            return;

        Ioc.Default.GetRequiredService<IDispatcherService>().RunOnUIThread(() =>
        {
            TrackChild(child);
        });
    }
}
