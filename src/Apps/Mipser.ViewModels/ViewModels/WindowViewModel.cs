// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Files;
using Mipser.Messages.Navigation;
using Mipser.ViewModels.Views;

namespace Mipser.ViewModels;

/// <summary>
/// The view model for the root window.
/// </summary>
public class WindowViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;
    private bool _isCheatSheetOpen;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
    /// </summary>
    /// <param name="messenger"></param>
    public WindowViewModel(IMessenger messenger)
    {
        _messenger = messenger;

        CreateNewFileCommand = new RelayCommand(CreateNewFile);
        PickAndOpenFileCommand = new RelayCommand(PickAndOpenFile);
        PickAndOpenFolderCommand = new RelayCommand(PickAndOpenFolder);
        CloseFileCommand = new RelayCommand(CloseFile);
        OpenCheatSheetCommand = new RelayCommand(OpenCheatSheet);

        IsActive = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the cheat sheet is open.
    /// </summary>
    public bool IsCheatSheetOpen
    {
        get => _isCheatSheetOpen;
        set => SetProperty(ref _isCheatSheetOpen, true);
    }

    /// <summary>
    /// Gets a command that creates and opens an anonymous file.
    /// </summary>
    public RelayCommand CreateNewFileCommand { get; }

    /// <summary>
    /// Gets a command that picks and opens a file.
    /// </summary>
    public RelayCommand PickAndOpenFileCommand { get; }

    /// <summary>
    /// Gets a command that picks and opens a folder.
    /// </summary>
    public RelayCommand PickAndOpenFolderCommand { get; }

    /// <summary>
    /// Gets a command that closes the currently open file.
    /// </summary>
    public RelayCommand CloseFileCommand { get; }

    /// <summary>
    /// Gets a command that closes the currently open file.
    /// </summary>
    public RelayCommand OpenCheatSheetCommand { get; }
    
    /// <inheritdoc/>
    protected override void OnActivated()
    {
        _messenger.Register<WindowViewModel, OpenCheatSheetRequestMessage>(this, (r, m) => IsCheatSheetOpen = true);
    }

    private void CreateNewFile() => _messenger.Send(new FileCreateNewRequestMessage());

    private void PickAndOpenFile() => _messenger.Send(new FilePickAndOpenRequestMessage());

    private void PickAndOpenFolder() => _messenger.Send(new FolderPickAndOpenRequestMessage());

    private void CloseFile() => _messenger.Send(new FileCloseRequestMessage());

    private void OpenCheatSheet() => _messenger.Send(new OpenCheatSheetRequestMessage());
}
