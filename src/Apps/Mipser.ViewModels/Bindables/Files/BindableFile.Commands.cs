// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mipser.Messages.Navigation;
using Mipser.Services;

namespace Mipser.Bindables.Files;

public partial class BindableFile
{
    /// <summary>
    /// Gets a command to open the file.
    /// </summary>
    public RelayCommand OpenCommand { get; }

    /// <summary>
    /// Open the file.
    /// </summary>
    public void Open() => Service.Get<IMessenger>().Send(new FileOpenRequestMessage(this));
}
