// Avishai Dernis 2025

using Mipser.Bindables.Files;

namespace Mipser.Messages.Navigation;

/// <summary>
/// A message sent requesting to open a file.
/// </summary>
public class FileOpenRequestMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileOpenRequestMessage"/> class.
    /// </summary>
    public FileOpenRequestMessage(BindableFile file)
    {
        File = file;
    }

    /// <summary>
    /// Gets the file to open.
    /// </summary>
    public BindableFile File { get; }
}
