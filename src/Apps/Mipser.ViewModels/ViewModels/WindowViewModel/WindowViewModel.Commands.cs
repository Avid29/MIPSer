// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Build;
using Mipser.Messages.Files;
using Mipser.Messages.Pages;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.App;
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
    public RelayCommand ClosePageCommand { get; }

    /// <summary>
    /// Gets a command that assembles the current file.
    /// </summary>
    public RelayCommand AssembleFileCommand { get; }

    /// <summary>
    /// Gets a command that opens the about page.
    /// </summary>
    public RelayCommand OpenAboutCommand { get; }

    /// <summary>
    /// Gets a command that opens the cheat sheet.
    /// </summary>
    public RelayCommand OpenCheatSheetCommand { get; }

    /// <summary>
    /// Gets a command that opens the create project page.
    /// </summary>
    public RelayCommand OpenCreateProjectCommand { get; }

    /// <summary>
    /// Gets a command that opens the settings page.
    /// </summary>
    public RelayCommand OpenSettingsCommand { get; }

    /// <summary>
    /// Gets a command that opens the welcome page.
    /// </summary>
    public RelayCommand OpenWelcomeCommand { get; }

    private void CreateNewFile() => _messenger.Send(new FileCreateNewRequestMessage());

    private void SaveFile() => _messenger.Send(new FileSaveRequestMessage());

    private async Task PickAndOpenFileAsync() => await MainViewModel.PickAndOpenFileAsync();

    private async Task PickAndOpenFolderAsync() => await MainViewModel.PickAndOpenFolderAsync();

    private async Task PickAndOpenProjectAsync() => await MainViewModel.PickAndOpenProjectAsync();

    private void ClosePage() => _messenger.Send(new PageCloseRequestMessage());

    private void AssembleFile()
    {
        // Get the current page, and ensure it's a file page
        if (MainViewModel.FocusedPanel?.CurrentPage is not FilePageViewModel page)
            return;

        // Check if the file is null
        var file = page.File?.File;
        if (file is null)
            return;

        // Request to assemble the file
        _messenger.Send(new AssembleFilesRequestMessage([file]));
    }

    private void OpenAbout() => MainViewModel.GoToPageByType<AboutPageViewModel>();

    private void OpenCheatSheet() => MainViewModel.GoToPageByType<CheatSheetViewModel>();

    private void OpenCreateProject() => MainViewModel.GoToPageByType<CreateProjectViewModel>();

    private void OpenSettings() => MainViewModel.GoToPageByType<SettingsPageViewModel>();

    private void OpenWelcome() => MainViewModel.GoToPageByType<WelcomePageViewModel>();
}
