// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages;
using Mipser.Messages.Files;
using Mipser.Models;
using Mipser.Services;
using Mipser.Services.Files;
using Mipser.Services.Files.Models;
using Mipser.Services.Settings;
using Mipser.ViewModels.Pages.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for the explorer.
/// </summary>
public class ExplorerViewModel : PageViewModel
{
    private const string RecentProjectsCacheKey = "RecentProjects";

    private readonly IMessenger _messenger;
    private readonly ICacheService _cacheService;
    private readonly IFileService _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExplorerViewModel"/> class.
    /// </summary>
    public ExplorerViewModel(IMessenger messenger, ICacheService cacheService, IFileService fileService)
    {
        _fileService = fileService;
        _cacheService = cacheService;
        _messenger = messenger;

        RecentProjects = [];

        _ = LoadRecentCacheAsync();

        IsActive = true;
    }

    /// <inheritdoc/>
    public override string Title => "Explorer"; // TODO: Localization

    /// <summary>
    /// Gets or sets the currently selected file.
    /// </summary>
    public BindableFileItem? RootItem
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                OnPropertyChanged(nameof(RootNode));
        }
    }

    /// <summary>
    /// Gets the list of root nodes.
    /// </summary>
    public IEnumerable<BindableFileItem?> RootNode => [RootItem];

    /// <summary>
    /// Gets a list of the recently opened projects and folders.
    /// </summary>
    public ObservableCollection<string> RecentProjects { get; private set; }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<ExplorerViewModel, FolderOpenedMessage>(this, async (r, m) =>
        {
            var folder = m.Folder;
            if (folder is null)
            {
                RootItem = null;
                return;
            }

            r.RootItem = await _fileService.GetFolderAsync(folder.Path);
        });
        _messenger.Register<ExplorerViewModel, CacheChangedMessage<RecentFileItemsCache>>(this, async (r, m) => await r.LoadRecentCacheAsync());
    }

    private async Task LoadRecentCacheAsync()
    {
        // Get current cache
        var recent = await _cacheService.RetrieveCacheAsync<RecentFileItemsCache>(RecentProjectsCacheKey);
        if (recent is null)
            return;

        RecentProjects.Clear();
        foreach(var item in recent.Paths)
            RecentProjects.Add(item);
    }
}
