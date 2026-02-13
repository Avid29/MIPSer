// Avishai Dernis 2025

using System.Collections.Generic;
using System.Threading.Tasks;
using Zarem.Assembler.Logging;
using Zarem.Config;
using Zarem.Models;
using Zarem.Models.Files;

namespace Zarem;

/// <summary>
/// A loaded mipser project.
/// </summary>
public interface IProject
{
    /// <summary>
    /// Gets the collection of source files in the project.
    /// </summary>
    public abstract SourceCollection SourceFiles { get; }

    /// <summary>
    /// Gets the project's configuration.
    /// </summary>
    public IProjectConfig Config { get; }

    /// <summary>
    /// Builds the project.
    /// </summary>
    public Task<BuildResult?> BuildProjectAsync(bool rebuild = false, Logger? logger = null);

    /// <summary>
    /// Assembles a list of files.
    /// </summary>
    public Task<BuildResult?> AssembleFilesAsync(IEnumerable<SourceFile> files, bool rebuild = true, Logger? logger = null);

    /// <summary>
    /// Cleans all files in the project.
    /// </summary>
    void CleanProject() => CleanFiles(SourceFiles);

    /// <summary>
    /// Cleans a list of source files.
    /// </summary>
    /// <param name="files">The files to clean.</param>
    void CleanFiles(IEnumerable<SourceFile> files);

    /// <summary>
    /// Saves the project configuration.
    /// </summary>
    void Save();
}
