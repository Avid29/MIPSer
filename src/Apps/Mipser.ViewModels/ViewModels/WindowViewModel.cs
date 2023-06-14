// Adam Dernis 2023

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Bindables;
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

        OpenFiles = new ObservableCollection<BindableFile>();

        CreateAnonymousFileCommand = new RelayCommand(CreateAnonymousFile);
    }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of open files.
    /// </summary>
    public ObservableCollection<BindableFile> OpenFiles { get; }

    /// <summary>
    /// Gets a command that creates and opens an anonymous file.
    /// </summary>
    public RelayCommand CreateAnonymousFileCommand { get; }

    /// <summary>
    /// Creates and opens a new anonymous file.
    /// </summary>
    public void CreateAnonymousFile() => OpenFiles.Add(new BindableFile());

    /// <summary>
    /// Closes a file.
    /// </summary>
    /// <remarks>
    /// Does not save the file.
    /// </remarks>
    /// <param name="file"></param>
    public void CloseFile(BindableFile file) => OpenFiles.Remove(file);
}
