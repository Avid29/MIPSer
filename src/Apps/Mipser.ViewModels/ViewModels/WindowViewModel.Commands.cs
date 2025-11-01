// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Files;
using Mipser.Messages.Navigation;
using Mipser.Messages.Pages;

namespace Mipser.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets or sets the main <see cref="PanelViewModel"/> for the window.
    /// </summary>
    public PanelViewModel MainPanel { get; }

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

    private void CreateNewFile() => _messenger.Send(new FileCreateNewRequestMessage());

    private void PickAndOpenFile() => _messenger.Send(new FilePickAndOpenRequestMessage());

    private void PickAndOpenFolder() => _messenger.Send(new FolderPickAndOpenRequestMessage());

    private void CloseFile() => _messenger.Send(new PageCloseRequestMessage());

    private void OpenCheatSheet() => _messenger.Send(new OpenCheatSheetRequestMessage());
}
