// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using Mipser.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mipser.Bindables.Files;

public partial class BindableFolder
{
    /// <summary>
    /// Gets a command to open the folder in the file explorer.
    /// </summary>
    public RelayCommand OpenInExplorerCommand { get; }
    
    /// <summary>
    /// Gets a command to open the folder in the windows terminal.
    /// </summary>
    public RelayCommand OpenInWindowsTerminalCommand { get; }

    /// <summary>
    /// Open the windows file explorer to the folder.
    /// </summary>
    public void OpenInExplorer() => Process.Start("explorer.exe", $"\"{Path}\"");

    /// <summary>
    /// Open the windows terminal to the folder.
    /// </summary>
    public void OpenInWindowsTerminal() => Process.Start("wt.exe", $"-d \"{Path}\"");

    /// <inheritdoc/>
    public override Task CopyFileAsync() => Service.Get<IClipboardService>().CopyFileItemsAsync([FileItem]);
}
