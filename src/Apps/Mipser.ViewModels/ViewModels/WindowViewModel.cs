// Adam Dernis 2024

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Navigation;

namespace Mipser.ViewModels;

/// <summary>
/// The view model for the root window.
/// </summary>
public partial class WindowViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
    /// </summary>
    public WindowViewModel(MainViewModel mainViewModel, PanelViewModel panelViewModel, IMessenger messenger)
    {
        _messenger = messenger;
        MainViewModel = mainViewModel;
        MainPanelViewModel = panelViewModel;

        CreateNewFileCommand = new RelayCommand(CreateNewFile);
        SaveFileCommand = new RelayCommand(SaveFile);
        PickAndOpenFileCommand = new RelayCommand(PickAndOpenFile);
        PickAndOpenFolderCommand = new RelayCommand(PickAndOpenFolder);
        ClosePageCommand = new RelayCommand(ClosePage);

        AssembleFileCommand = new RelayCommand(AssembleFile);

        OpenAboutCommand = new RelayCommand(OpenAbout);
        OpenCheatSheetCommand = new RelayCommand(OpenCheatSheet);
        OpenSettingsCommand = new RelayCommand(OpenSettings);

        IsActive = true;

        // Notify that the main panel is focused on startup
        _messenger.Send(new PanelFocusChangedMessage(MainPanelViewModel));
    }

    /// <summary>
    /// Gets the <see cref="MainViewModel"/> for the app.
    /// </summary>
    public MainViewModel MainViewModel { get; }

    /// <summary>
    /// Gets the <see cref="PanelViewModel"/> for the main panel in the window.
    /// </summary>
    public PanelViewModel MainPanelViewModel { get; }
}
