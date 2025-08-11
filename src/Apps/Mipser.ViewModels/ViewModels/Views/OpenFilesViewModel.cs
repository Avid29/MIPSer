// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Files;
using Mipser.Services.Files;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Mipser.ViewModels.Views;

/// <summary>
/// A view model for tracking the open files.
/// </summary>
public class OpenFilesViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;
    private readonly IFilesService _fileService;

    private BindableFile? _currentFile;
     
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFilesViewModel"/> class.
    /// </summary>
    public OpenFilesViewModel(IMessenger messenger, IFilesService filesService)
    {
        _messenger = messenger;
        _fileService = filesService;

        OpenFiles = [];
        IsActive = true;
    }

    /// <summary>
    /// Gets or sets the currently selected file.
    /// </summary>
    public BindableFile? CurrentFile
    {
        get => _currentFile;
        set => SetProperty(ref _currentFile, value);
    }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of open files.
    /// </summary>
    public ObservableCollection<BindableFile> OpenFiles { get; }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<OpenFilesViewModel, FileCreateNewRequestMessage>(this, (r, m) => r.CreateNewFile());
        _messenger.Register<OpenFilesViewModel, FilePickAndOpenRequestMessage>(this, (r, m) => _ = r.PickAndOpenFileAsync());
        _messenger.Register<OpenFilesViewModel, FileCloseRequestMessage>(this, (r, m) => r.CloseFile(m.File));
    }

    /// <summary>
    /// Creates and opens a new anonymous file.
    /// </summary>
    private void CreateNewFile() => OpenFiles.Add(new BindableFile());

    /// <summary>
    /// Picks and opens a file.
    /// </summary>
    private async Task PickAndOpenFileAsync()
    {
        var file = await _fileService.TryPickAndOpenFileAsync();
        if (file is null)
            return;

        var bindable = new BindableFile(file);
        OpenFiles.Add(bindable);
    }

    /// <summary>
    /// Closes a file.
    /// </summary>
    /// <remarks>
    /// Does not save the file.
    /// </remarks>
    /// <param name="file"></param>
    private void CloseFile(BindableFile? file)
    {
        file ??= CurrentFile;
        if (file is null)
            return;

        OpenFiles.Remove(file);
    }
}
