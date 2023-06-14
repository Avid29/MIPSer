// Adam Dernis 2023

using Mipser.Bindables;

namespace Mipser.Messages.Files;

/// <summary>
/// A message sent to close a file.
/// </summary>
public sealed class FileCloseRequestMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileCloseRequestMessage"/> class.
    /// </summary>
    public FileCloseRequestMessage(BindableFile file)
    {
        File = file;
    }

    /// <summary>
    /// Gets the file to close.
    /// </summary>
    public BindableFile File { get; }
}
