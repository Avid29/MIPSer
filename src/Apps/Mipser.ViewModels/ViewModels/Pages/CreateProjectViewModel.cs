// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MIPS.Assembler.Models.Config;
using MIPS.Models.Instructions.Enums;
using Mipser.Messages.Pages;
using Mipser.Models.ProjectConfig;
using Mipser.Services.Files;
using Mipser.Services.Localization;
using Mipser.ViewModels.Pages.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Mipser.ViewModels.Pages;

/// <summary>
/// A view model for a page to create a new project.
/// </summary>
public class CreateProjectViewModel : PageViewModel
{
    private readonly IMessenger _messenger;
    private readonly ILocalizationService _localizationService;
    private readonly IFileSystemService _fileSystemService;

    private string? _projectName;
    private string? _folderPath;
    private MipsVersion _mipsVersion = MipsVersion.MipsIII;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheatSheetViewModel"/> class.
    /// </summary>
    public CreateProjectViewModel(IMessenger messenger, ILocalizationService localizationService, IFileSystemService fileSystemService)
    {
        _messenger = messenger;
        _localizationService = localizationService;
        _fileSystemService = fileSystemService;

        SelectFolderCommand = new(SelectFolderAsync);
        CreateProjectCommand = new(CreateProjectAsync);
        CancelCommand = new(Cancel);
    }

    /// <inheritdoc/>
    public override string Title => _localizationService["/PageTitles/CreateNewProject"];

    /// <summary>
    /// Gets or sets the name of the project to create.
    /// </summary>
    public string? ProjectName
    {
        get => _projectName;
        set
        {
            if(SetProperty(ref _projectName, value))
            {
                OnPropertyChanged(nameof(ReadyToCreate));
            }
        }
    }

    /// <summary>
    /// Gets or sets the path of the folder to create the folder in.
    /// </summary>
    public string? FolderPath
    {
        get => _folderPath;
        set
        {
            if (SetProperty(ref _folderPath, value))
            {
                OnPropertyChanged(nameof(ReadyToCreate));
            }
        }
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
    /// Gets whether or not the project can be created.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ProjectName), nameof(FolderPath))]
    public bool ReadyToCreate => ProjectName is not null && FolderPath is not null;

    /// <summary>
    /// Gets a command that selects a folder for the folder path.
    /// </summary>
    public AsyncRelayCommand SelectFolderCommand { get; }

    /// <summary>
    /// Gets a command that creates the project
    /// </summary>
    public AsyncRelayCommand CreateProjectCommand { get; }

    /// <summary>
    /// Gets a command that cancels creating a new project
    /// </summary>
    public RelayCommand CancelCommand { get; }

    private async Task CreateProjectAsync()
    {
        // TODO: Notify errors

        if (!ReadyToCreate)
            return;

        // Attempt to create the project root folder
        var rootFolderPath = Path.Combine(FolderPath, ProjectName);
        var rootFolder = await _fileSystemService.CreateFolderAsync(rootFolderPath);
        if (rootFolder is null)
            return;

        // Attempt to create project file
        var projectFilePath = Path.Combine(rootFolderPath, $"{ProjectName}.mipsproj");
        var projectFile = await _fileSystemService.CreateFileAsync(projectFilePath);
        if (projectFile is null)
            return;

        // Attempt to open the project file for writing
        var stream = await projectFile.OpenStreamForWriteAsync();
        if (stream is null)
            return;

        // Create the file config
        var projectConfig = new ProjectConfig
        {
            Name = ProjectName,
            Path = new Uri(rootFolderPath),
            AssemblerConfig = new AssemblerConfig(MipsVersion)
        };

        // Write project config to the file 
        projectConfig.Serialize(stream);

        // TODO: Open the project and close the page
        // TODO: Open the project in a new window
    }

    private async Task SelectFolderAsync()
    {
        var folder = await _fileSystemService.PickFolderAsync();
        if (folder is null)
            return;

        FolderPath = folder.Path;
    }

    private void Cancel() => _messenger.Send(new PageCloseRequestMessage(this));
}
