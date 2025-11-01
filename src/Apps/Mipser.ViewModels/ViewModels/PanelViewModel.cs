// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Messages.Pages;
using Mipser.Services.Files;
using Mipser.ViewModels.Views;
using Mipser.ViewModels.Views.Abstract;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Mipser.ViewModels;

/// <summary>
/// A view model for tracking the open files.
/// </summary>
public class PanelViewModel : ObservableObject
{
    private readonly IMessenger _messenger;
    private readonly IFilesService _fileService;

    private PageViewModel? _currentPage;
     
    /// <summary>
    /// Initializes a new instance of the <see cref="PanelViewModel"/> class.
    /// </summary>
    public PanelViewModel(IMessenger messenger, IFilesService filesService)
    {
        _messenger = messenger;
        _fileService = filesService;

        OpenPages = [];
    }

    /// <summary>
    /// Gets or sets the currently selected file.
    /// </summary>
    public PageViewModel? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of open files.
    /// </summary>
    public ObservableCollection<PageViewModel> OpenPages { get; }

    /// <summary>
    /// Creates and opens a new anonymous file.
    /// </summary>
    public void CreateNewFile() => OpenPages.Add(new FilePageViewModel());

    /// <summary>
    /// Picks and opens a file.
    /// </summary>
    public async Task PickAndOpenFileAsync()
    {
        var file = await _fileService.TryPickAndOpenFileAsync();
        if (file is null)
            return;

        var bindable = new BindableFile(file);
        OpenPages.Add(new FilePageViewModel(bindable));
    }

    /// <summary>
    /// Closes a file.
    /// </summary>
    /// <remarks>
    /// Does not save the file.
    /// </remarks>
    public void ClosePage(PageViewModel? file)
    {
        file ??= CurrentPage;
        if (file is null)
            return;

        OpenPages.Remove(file);
    }
}
