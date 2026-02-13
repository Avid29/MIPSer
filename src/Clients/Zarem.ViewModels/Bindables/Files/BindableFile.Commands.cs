// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Zarem.Messages.Navigation;
using Zarem.Services;
using System.Threading.Tasks;

namespace Zarem.Bindables.Files;

public partial class BindableFile
{
    /// <summary>
    /// Open the file.
    /// </summary>
    [RelayCommand]
    public void Open() => Service.Get<IMessenger>().Send(new FileOpenRequestMessage(this));

    /// <inheritdoc/>
    public override async Task CopyFileAsync() => await Service.Get<IClipboardService>().CopyFileItemsAsync([FileItem]);
}
