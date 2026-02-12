// Avishai Dernis 2025

using System.IO;

namespace Zarem.Models.Files;

/// <summary>
/// A base class for a file in a project.
/// </summary>
public abstract class FileBase : ProjectItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileBase"/> class.
    /// </summary>
    public FileBase(Project project, string fullPath) : base(project)
    {
        FullPath = fullPath;
    }

    /// <summary>
    /// Gets the name of the source file without its extension.
    /// </summary>
    public string Name => Path.GetFileNameWithoutExtension(FullPath);

    /// <summary>
    /// Gets the path to the file relative to the collection root.
    /// </summary>
    public string RelativePath => Path.GetRelativePath(Collection.RootPath, FullPath);

    /// <summary>
    /// Gets the full path of the file.
    /// </summary>
    public string FullPath { get; internal set; }

    /// <summary>
    /// Gets the <see cref="SourceCollection"/> the file belongs to.
    /// </summary>
    public SourceCollection Collection => Project.SourceFiles;
}
