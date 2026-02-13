// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using Zarem.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zarem.Bindables.Files;

public partial class BindableFolder
{
    /// <summary>
    /// Open the windows file explorer to the folder.
    /// </summary>
    [RelayCommand]
    public void OpenInExplorer() => Process.Start("explorer.exe", $"\"{Path}\"");

    /// <summary>
    /// Open the windows terminal to the folder.
    /// </summary>
    [RelayCommand]
    public void OpenInWindowsTerminal() => Process.Start("wt.exe", $"-d \"{Path}\"");

    /// <inheritdoc/>
    public override Task CopyFileAsync() => Service.Get<IClipboardService>().CopyFileItemsAsync([FileItem]);
}
