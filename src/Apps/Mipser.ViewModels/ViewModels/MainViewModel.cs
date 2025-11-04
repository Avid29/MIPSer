// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.ViewModels.Pages;

namespace Mipser.ViewModels;

/// <summary>
/// The main view model for the application.
/// </summary>
public partial class MainViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;

    private PanelViewModel? _focusPanel;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel(IMessenger messenger)
    {
        _messenger = messenger;
        IsActive = true;
    }

    /// <summary>
    /// Gets the currently focused <see cref="PanelViewModel"/>.
    /// </summary>
    public PanelViewModel? FocusedPanel
    {
        get => _focusPanel;
        private set
        {
            if(SetProperty(ref _focusPanel, value))
            {
                OnPropertyChanging(nameof(FocusedPanel));
            }
        }
    }
    
    /// <summary>
    /// Gets the currently open <see cref="FilePageViewModel"/>, or null if the current page is not a file.
    /// </summary>
    public FilePageViewModel? CurrentFilePage => FocusedPanel?.CurrentPage as FilePageViewModel;
}
