// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using Zarem.ViewModels.Pages;
using Zarem.ViewModels.Pages.App;

namespace Zarem.ViewModels;

public partial class WindowViewModel
{
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

    /// <summary>
    /// Gets a command that toggles full screen mode.
    /// </summary>
    public RelayCommand ToggleFullScreenModeCommand { get; }

    /// <summary>
    /// Gets a command that shows the console.
    /// </summary>
    public RelayCommand ShowConsoleCommand { get; }

    private void OpenAbout() => MainViewModel.GoToPageByType<AboutPageViewModel>();

    private void OpenCheatSheet() => MainViewModel.GoToPageByType<CheatSheetViewModel>();

    private void OpenCreateProject() => MainViewModel.GoToPageByType<CreateProjectViewModel>();

    private void OpenSettings() => MainViewModel.GoToPageByType<SettingsPageViewModel>();

    /// <summary>
    /// Open the welcome page.
    /// </summary>
    public void OpenWelcome() => MainViewModel.GoToPageByType<WelcomePageViewModel>();

    private void ShowConsole() => _consoleService.ShowConsoleWindow();

    private void ToggleFullscrenMode() => _windowingService.ToggleFullScreen();
}
