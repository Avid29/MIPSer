// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Services.Files;
using Mipser.ViewModels.Pages.Abstract;
using System.Collections.Generic;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for the explorer.
/// </summary>
public class ExplorerViewModel : PageViewModel
{
    private readonly IMessenger _messenger;
    private readonly IFileService _fileService;

    private BindableFolder? _rootFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExplorerViewModel"/> class.
    /// </summary>
    public ExplorerViewModel(IMessenger messenger, IFileService fileService)
    {
        _fileService = fileService;
        _messenger = messenger;

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
                OnPropertyChanged(nameof(RootNode));
        }
    }

    /// <summary>
    /// Gets the list of root nodes.
    /// </summary>
    public IEnumerable<BindableFileItem?> RootNode => [RootFolder];

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<ExplorerViewModel, FolderOpenedMessage>(this, async (r, m) =>
        {
            var folder = m.Folder;
            if (folder is null)
            {
                RootFolder = null;
                return;
            }

            r.RootFolder = await _fileService.GetFolderAsync(folder.Path);
        });
    }
}
