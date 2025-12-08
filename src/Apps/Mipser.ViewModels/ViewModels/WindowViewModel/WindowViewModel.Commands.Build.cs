// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Build;
using Mipser.Messages.Files;
using Mipser.Messages.Pages;
using Mipser.Services;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.App;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Mipser.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets a command that builds the project.
    /// </summary>
    public AsyncRelayCommand BuildProjectCommand { get; }

    /// <summary>
    /// Gets a command that builds the project.
    /// </summary>
    public AsyncRelayCommand RebuildProjectCommand { get; }

    /// <summary>
    /// Gets a command that assembles the currently open files.
    /// </summary>
    public AsyncRelayCommand AssembleOpenFilesCommand { get; }

    /// <summary>
    /// Gets a command that assembles the current file.
    /// </summary>
    public AsyncRelayCommand AssembleFileCommand { get; }

    /// <summary>
    /// Gets a command that cleans the project.
    /// </summary>
    public RelayCommand CleanProjectCommand { get; }

    /// <summary>
    /// Gets a command that cleans the current open files.
    /// </summary>
    public RelayCommand CleanOpenFilesCommand { get; }

    /// <summary>
    /// Gets a command that cleans the current file.
    /// </summary>
    public RelayCommand CleanFileCommand { get; }

    private async Task BuildProjectAsync() => await _buildService.BuildProjectAsync();

    private async Task RebuildProjectAsync() => await _buildService.BuildProjectAsync(true);

    private async Task AssembleOpenFilesAsync()
    {
        var openSourceFiles = OpenFiles
            .Where(x => x.SourceFile is not null)
            .Select(x => x.SourceFile!);

        await _buildService.AssembleFilesAsync(openSourceFiles);
    }

    private async Task AssembleFileAsync()
    {
        // Check if the file is null
        var file = CurrentFile?.SourceFile;
        if (file is null)
            return;

        // Request to assemble the file
        await _buildService.AssembleFilesAsync([file]);
    }

    private void CleanProject() => _buildService.CleanProject();

    private void CleanOpenFiles()
    {
        var openSourceFiles = OpenFiles
            .Where(x => x.SourceFile is not null)
            .Select(x => x.SourceFile!);

        _buildService.CleanFiles(openSourceFiles);
    }

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
