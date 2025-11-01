// Adam Dernis 2024

namespace Mipser.Messages.Files;

/// <summary>
/// A message sent to save a file.
/// </summary>
public sealed class FileSaveRequestMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSaveRequestMessage"/> class.
    /// </summary>
    public FileSaveRequestMessage(bool saveAsNewFile = false)
    {
        SaveAsNewFile = saveAsNewFile;
    }

    /// <summary>
    /// Gets if the file should be saved as a new file
    /// </summary>
    public bool SaveAsNewFile { get; }
}
