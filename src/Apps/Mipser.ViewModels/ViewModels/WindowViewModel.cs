// Adam Dernis 2023

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables;
using Mipser.Messages.Files;
using System.Collections.ObjectModel;

namespace Mipser.ViewModels;

/// <summary>
/// The view model for the root window.
/// </summary>
public class WindowViewModel : ObservableRecipient
{
    private readonly IMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
    /// </summary>
    /// <param name="messenger"></param>
    public WindowViewModel(IMessenger messenger)
    {
        _messenger = messenger;

        CreateNewFileCommand = new RelayCommand(CreateNewFile);
        PickAndOpenFileCommand = new RelayCommand(PickAndOpenFile);
    }

    /// <summary>
    /// Gets a command that creates and opens an anonymous file.
    /// </summary>
    public RelayCommand CreateNewFileCommand { get; }

    /// <summary>
    /// Gets a command that creates and opens an anonymous file.
    /// </summary>
    public RelayCommand PickAndOpenFileCommand { get; }

    private void CreateNewFile() => _messenger.Send(new FileCreateNewRequestMessage());

    private void PickAndOpenFile() => _messenger.Send(new FilePickAndOpenRequestMessage());
}
