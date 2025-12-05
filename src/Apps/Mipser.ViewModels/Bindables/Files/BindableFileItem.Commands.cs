// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using Mipser.Services;

namespace Mipser.Bindables.Files;

public partial class BindableFileItem
{
    /// <summary>
    /// Gets a command to copy the file name to the clipboard.
    /// </summary>
    public RelayCommand CopyFileNameCommand { get; }

    /// <summary>
    /// Gets a command to copy the file path to the clipboard.
    /// </summary>
    public RelayCommand CopyFilePathCommand { get; }

    /// <summary>
    /// Copies the file name to the clipboard.
    /// </summary>
    public void CopyFileName() => Service.Get<IClipboardService>().Copy(Name);

    /// <summary>
    /// Copies the file's path to the clipboard.
    /// </summary>
    public void CopyFilePath() => Service.Get<IClipboardService>().Copy(Path);
}
