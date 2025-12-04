// Avishai Dernis 2025

using Mipser.Services.Files.Models;

namespace Mipser.Messages.Build;

/// <summary>
/// A message sent to request assembling the current file.
/// </summary>
public class AssembleFilesRequestMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssembleFilesRequestMessage"/> class.
    /// </summary>
    public AssembleFilesRequestMessage(IFile[] files)
    {
        Files = files;
    }

    /// <summary>
    /// Gets the files requested to assemble.
    /// </summary>
    public IFile[] Files { get; }
}
