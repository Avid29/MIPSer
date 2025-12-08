// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables.Files;
using Mipser.Messages.Navigation;
using Mipser.Models.Files;
using Mipser.Services;
using Mipser.ViewModels.Pages;

namespace Mipser.ViewModels;

/// <summary>
/// The view model for the root window.
/// </summary>
public partial class WindowViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;
    private readonly IProjectService _projectService;
    private readonly BuildService _buildService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
    /// </summary>
    public WindowViewModel(IMessenger messenger, IProjectService projectService, BuildService buildService, MainViewModel mainViewModel, PanelViewModel panelViewModel)
    {
        _messenger = messenger;
        _projectService = projectService;
        _buildService = buildService;

        MainViewModel = mainViewModel;
        PanelViewModel = panelViewModel;

        CreateNewFileCommand = new(CreateNewFile);
        SaveFileCommand = new(SaveFile);
        PickAndOpenFileCommand = new(PickAndOpenFileAsync);
        PickAndOpenFolderCommand = new(PickAndOpenFolderAsync);
        PickAndOpenProjectCommand = new(PickAndOpenProjectAsync);
        ClosePageCommand = new(ClosePageAsync);
        CloseProjectCommand = new(CloseProjectAsync);

        BuildProjectCommand = new(BuildProjectAsync);
        RebuildProjectCommand = new(RebuildProjectAsync);
        AssembleFileCommand = new(AssembleFileAsync);
        CleanProjectCommand = new(CleanProject);
        CleanFileCommand = new(CleanFile);

        OpenAboutCommand = new(OpenAbout);
        OpenCheatSheetCommand = new(OpenCheatSheet);
        OpenCreateProjectCommand = new(OpenCreateProject);
        OpenSettingsCommand = new(OpenSettings);
        OpenWelcomeCommand = new(OpenWelcome);

        UndoCommand = new(Undo);
        RedoCommand = new(Redo);
        CutCommand = new(Cut);
        CopyCommand = new(Copy);
        PasteCommand = new(Paste);
        DuplicateCommand = new(Duplicate);
        SelectAllCommand = new(SelectAll);

        TransposeUpCommand = new(TransposeUp);
        TransposeDownCommand = new(TransposeDown);
        
        ToggleOutliningCommand = new(ToggleOutlining);
        ExpandCurrentCommand = new(ExpandChildren);
        CollapseCurrentCommand = new(CollapseChildren);
        ExpandAllCommand = new(ExpandAll);
        CollapseAllCommand = new(CollapseAll);

        IsActive = true;

        // Focus the panel when the window is created.
        _messenger.Send(new PanelFocusChangedMessage(PanelViewModel));

    }

    /// <summary>
    /// Gets the <see cref="MainViewModel"/> for the app.
    /// </summary>
    public MainViewModel MainViewModel { get; }

    /// <summary>
    /// Gets the <see cref="ViewModels.PanelViewModel"/> for the panel in the window.
    /// </summary>
    public PanelViewModel PanelViewModel { get; }

    private BindableFile? CurrentFile
    {
        get
        {
            // Get the current page, and ensure it's a file page
            if (MainViewModel.FocusedPanel?.CurrentPage is not FilePageViewModel page)
                return null;

            return page.File;
        }
    }
}
