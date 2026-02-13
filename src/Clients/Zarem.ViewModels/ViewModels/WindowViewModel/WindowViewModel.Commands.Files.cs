// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Zarem.Messages.Files;
using System.Threading.Tasks;

namespace Zarem.ViewModels;

public partial class WindowViewModel
{
    [RelayCommand]
    private void CreateNewFile() => _messenger.Send(new FileCreateNewRequestMessage());

    [RelayCommand]
    private async Task SaveFileAsync() => MainViewModel.FocusedPanel?.SaveFileAsync();

    [RelayCommand]
    private async Task SaveAllFilesAsync() => await MainViewModel.SaveAllFilesAsync();

    [RelayCommand]
    private async Task PickAndOpenFileAsync() => await MainViewModel.PickAndOpenFileAsync();

    [RelayCommand]
    private async Task PickAndOpenFolderAsync() => await MainViewModel.PickAndOpenFolderAsync();

    [RelayCommand]
    private async Task PickAndOpenProjectAsync() => await MainViewModel.PickAndOpenProjectAsync();

    [RelayCommand]
    private async Task ClosePageAsync()
    {
        var panel = MainViewModel.FocusedPanel;
        if (panel is null)
            return;

        await panel.ClosePageAsync(null);
    }

    [RelayCommand]
    private async Task CloseProjectAsync() => await _projectService.CloseProjectAsync();
}
