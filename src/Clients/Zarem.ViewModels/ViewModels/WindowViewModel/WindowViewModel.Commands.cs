// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using Zarem.ViewModels.Pages;
using Zarem.ViewModels.Pages.App;

namespace Zarem.ViewModels;

public partial class WindowViewModel
{
    [RelayCommand]
    private void OpenAbout() => MainViewModel.GoToPageByType<AboutPageViewModel>();

    [RelayCommand]
    private void OpenMIPSCheatSheet() => MainViewModel.GoToPageByType<CheatSheetViewModel>();

    [RelayCommand]
    private void OpenCreateProject() => MainViewModel.GoToPageByType<CreateProjectViewModel>();

    [RelayCommand]
    private void OpenSettings() => MainViewModel.GoToPageByType<SettingsPageViewModel>();

    /// <summary>
    /// Open the welcome page.
    /// </summary>
    [RelayCommand]
    private void OpenWelcome() => MainViewModel.GoToPageByType<WelcomePageViewModel>();

    [RelayCommand]
    private void ShowConsole() => _consoleService.ShowConsoleWindow();

    [RelayCommand]
    private void ToggleFullscrenMode() => _windowingService.ToggleFullScreen();
}
