// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Services;
using Mipser.Services.Files;
using Mipser.ViewModels.Pages;

namespace Mipser.ViewModels;

/// <summary>
/// The main view model for the application.
/// </summary>
public partial class MainViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;
    private readonly IProjectService _projectService;
    private readonly BuildService _buildService;
    private readonly IFileService _fileService;
    private readonly IFileSystemService _fileSystemService;

    private PanelViewModel? _focusPanel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel(IMessenger messenger, IProjectService projectService, BuildService buildService, IFileService fileService, IFileSystemService fileSystemService, ExplorerViewModel explorerViewModel)
    {
        _messenger = messenger;
        _projectService = projectService;
        _buildService = buildService;
        _fileService = fileService;
        _fileSystemService = fileSystemService;

        ExplorerViewModel = explorerViewModel;

        IsActive = true;
    }

    /// <summary>
    /// Gets the currently focused <see cref="PanelViewModel"/>.
    /// </summary>
    public PanelViewModel? FocusedPanel
    {
        get => _focusPanel;
        private set => SetProperty(ref _focusPanel, value);
    }

    /// <summary>
    /// Gets the <see cref="ExplorerViewModel"/>.
    /// </summary>
    public ExplorerViewModel ExplorerViewModel { get; }
}
