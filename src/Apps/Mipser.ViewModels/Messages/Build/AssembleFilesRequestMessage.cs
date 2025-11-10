// Avishai Dernis 2025

using Mipser.Bindables.Files;

namespace Mipser.Messages.Build;

/// <summary>
/// A message sent to request assembling the current file.
/// </summary>
public class AssembleFilesRequestMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssembleFilesRequestMessage"/> class.
    /// </summary>
    public AssembleFilesRequestMessage(BindableFile[] files)
    {
        Files = files;
    }

    /// <summary>
    /// Gets the files requested to assemble.
    /// </summary>
    public BindableFile[] Files { get; }
}
