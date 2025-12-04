// Avishai Dernis 2025

namespace Mipser.Models;

/// <summary>
/// A model for a source file, including a paired object file.
/// </summary>
public class SourceFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceFile"/> class.
    /// </summary>
    public SourceFile(SourceCollection collection, string fullPath)
    {
        Collection = collection;
        FullPath = fullPath;
    }

    /// <summary>
    /// Gets the name of the source file without its extension.
    /// </summary>
    public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

    /// <summary>
    /// Gets the path to the file relative to the collection root.
    /// </summary>
    public string Path => System.IO.Path.GetRelativePath(Collection.RootPath, FullPath);

    /// <summary>
    /// Gets the full path of the file.
    /// </summary>
    public string FullPath { get; set; }

    /// <summary>
    /// Gets the <see cref="SourceCollection"/> the file belongs to.
    /// </summary>
    public SourceCollection Collection { get; }
}
