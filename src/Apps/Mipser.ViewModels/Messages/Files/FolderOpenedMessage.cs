// Avishai Dernis 2025

using Mipser.Bindables.Files;

namespace Mipser.Messages.Files;

/// <summary>
/// A message sent when a folder is opened.
/// </summary>
public class FolderOpenedMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FolderOpenedMessage"/> class.
    /// </summary>
    public FolderOpenedMessage(BindableFolder folder)
    {
        Folder = folder;
    }

    /// <summary>
    /// Gets the opened folder.
    /// </summary>
    public BindableFolder Folder { get; }
}
