// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Threading.Tasks;

namespace Zarem.ViewModels;

public partial class WindowViewModel
{
    [RelayCommand]
    private async Task BuildProjectAsync() => await _buildService.BuildProjectAsync();

    [RelayCommand]
    private async Task RebuildProjectAsync() => await _buildService.BuildProjectAsync(true);

    [RelayCommand]
    private async Task AssembleOpenFilesAsync()
    {
        var openSourceFiles = OpenFiles
            .Where(x => x.SourceFile is not null)
            .Select(x => x.SourceFile!);

        await _buildService.AssembleFilesAsync(openSourceFiles);
    }

    [RelayCommand]
    private async Task AssembleFileAsync()
    {
        // Check if the file is null
        var file = CurrentFile?.SourceFile;
        if (file is null)
            return;

        // Request to assemble the file
        await _buildService.AssembleFilesAsync([file]);
    }

    [RelayCommand]
    private void CleanProject() => _buildService.CleanProject();

    [RelayCommand]
    private void CleanOpenFiles()
    {
        var openSourceFiles = OpenFiles
            .Where(x => x.SourceFile is not null)
            .Select(x => x.SourceFile!);

        _buildService.CleanFiles(openSourceFiles);
    }

    [RelayCommand]
    private void CleanFile()
    {
        // Check if the file is null
        var file = CurrentFile?.SourceFile;
        if (file is null)
            return;

        // Request to assemble the file
        _buildService.CleanFiles([file]);
    }
}
