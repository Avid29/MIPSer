// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using System.IO;

namespace Mipser.Models.Files;

/// <summary>
/// A model for a source file with a reference to a paired object file.
/// </summary>
public class SourceFile : FileBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceFile"/> class.
    /// </summary>
    public SourceFile(Project project, string fullPath) : base(project, fullPath)
    {
        var directory = Path.GetDirectoryName(FullPath);
        Guard.IsNotNull(directory);

        var saveFileName = Name + ".obj";
        var saveFilePath = Path.Combine(directory, saveFileName);

        ObjectFile = new ObjectFile(this, saveFilePath);
    }

    /// <summary>
    /// Gets the associates object file.
    /// </summary>
    public ObjectFile ObjectFile { get; }
}
