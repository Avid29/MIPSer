// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Files;
using System.Threading.Tasks;

namespace Mipser.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets a command that creates and opens an anonymous file.
    /// </summary>
    public RelayCommand CreateNewFileCommand { get; }

    /// <summary>
    /// Gets the command that saves the current file.
    /// </summary>
    public RelayCommand SaveFileCommand { get; }

    /// <summary>
    /// Gets a command that picks and opens a file.
    /// </summary>
    public AsyncRelayCommand PickAndOpenFileCommand { get; }

    /// <summary>
    /// Gets a command that picks and opens a folder.
    /// </summary>
    public AsyncRelayCommand PickAndOpenFolderCommand { get; }

    /// <summary>
    /// Gets a command that picks and opens a project.
    /// </summary>
    public AsyncRelayCommand PickAndOpenProjectCommand { get; }

    /// <summary>
    /// Gets a command that closes the currently open page.
    /// </summary>
    public AsyncRelayCommand ClosePageCommand { get; }

    /// <summary>
    /// Gets a command that closes the currently open project.
    /// </summary>
    public AsyncRelayCommand CloseProjectCommand { get; }

    private void CreateNewFile() => _messenger.Send(new FileCreateNewRequestMessage());

    private void SaveFile() => _messenger.Send(new FileSaveRequestMessage());

    private async Task PickAndOpenFileAsync() => await MainViewModel.PickAndOpenFileAsync();

    private async Task PickAndOpenFolderAsync() => await MainViewModel.PickAndOpenFolderAsync();

    private async Task PickAndOpenProjectAsync() => await MainViewModel.PickAndOpenProjectAsync();

    private async Task ClosePageAsync()
    {
        var panel = MainViewModel.FocusedPanel;
        if (panel is null)
            return;

        await panel.ClosePageAsync(null);
    }

    private async Task CloseProjectAsync() => await _projectService.CloseProjectAsync();
}
