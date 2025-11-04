// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Bindables.Files.Abstract;
using Mipser.Messages.Files;
using Mipser.Services.FileSystem;
using Mipser.ViewModels.Pages.Abstract;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for the explorer.
/// </summary>
public class ExplorerViewModel : PageViewModel
{
    private readonly IMessenger _messenger;
    private readonly IFileSystemService _fileService;

    private BindableFolder? _rootFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExplorerViewModel"/> class.
    /// </summary>
    public ExplorerViewModel(IMessenger messenger, IFileSystemService filesService)
    {
        _messenger = messenger;
        _fileService = filesService;

        IsActive = true;
    }

    /// <inheritdoc/>
    public override string Title => "Explorer"; // TODO: Localization

    /// <summary>
    /// Gets or sets the currently selected file.
    /// </summary>
    public BindableFolder? RootFolder
    {
        get => _rootFolder;
        set
        {
            if (SetProperty(ref _rootFolder, value))
            {
                OnPropertyChanged(nameof(RootNodes));
            }
        }
    }

    /// <summary>
    /// Gets the list of root nodes.
    /// </summary>
    public ObservableCollection<BindableFilesItemBase>? RootNodes => RootFolder?.Children;

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<ExplorerViewModel, FolderPickAndOpenRequestMessage>(this, (r, m) => _ = r.PickAndOpenFolderAsync());
    }

    private async Task PickAndOpenFolderAsync()
    {
        var folder = await _fileService.TryPickAndOpenFolderAsync();
        if (folder is null)
            return;

        RootFolder = new BindableFolder(folder);
        await RootFolder.LoadChildrenAsync();
        
        OnPropertyChanged(nameof(RootFolder));
    }
}
