// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using MIPS.Models.Instructions.Enums;
using Mipser.Services.Files;
using Mipser.Services.Localization;
using Mipser.Services.Settings.Enums;
using Mipser.ViewModels.Pages.Abstract;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for a page to create a new project.
/// </summary>
public class CreateProjectViewModel : PageViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IFileSystemService _fileSystemService;

    private string? _projectName;
    private string? _folderPath;
    private MipsVersion _mipsVersion = MipsVersion.MipsIII;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheatSheetViewModel"/> class.
    /// </summary>
    public CreateProjectViewModel(ILocalizationService localizationService, IFileSystemService fileSystemService)
    {
        _localizationService = localizationService;
        _fileSystemService = fileSystemService;

        SelectFolderCommand = new(SelectFolderAsync);
    }

    /// <inheritdoc/>
    public override string Title => _localizationService["/PageTitles/CreateNewProject"];

    /// <summary>
    /// Gets or sets the name of the project to create.
    /// </summary>
    public string? ProjectName
    {
        get => _projectName;
        set => SetProperty(ref  _projectName, value);
    }

    /// <summary>
    /// Gets or sets the path of the folder to create the folder in.
    /// </summary>
    public string? FolderPath
    {
        get => _folderPath;
        set => SetProperty(ref  _folderPath, value);
    }

    /// <summary>
    /// Gets or sets the mips version for the project to create.
    /// </summary>
    public MipsVersion MipsVersion
    {
        get => _mipsVersion;
        set => SetProperty(ref _mipsVersion, value);
    }

    /// <summary>
    /// Gets the list of available mips version options.
    /// </summary>
    public IEnumerable<MipsVersion> MipsVersionOptions => Enum.GetValues<MipsVersion>();

    /// <summary>
    /// Gets a command that selects a folder for the folder path.
    /// </summary>
    public AsyncRelayCommand SelectFolderCommand { get; }

    private async Task SelectFolderAsync()
    {
        var folder = await _fileSystemService.PickFolderAsync();
        if (folder is null)
            return;

        FolderPath = folder.Path;
    }
}
