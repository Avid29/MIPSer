// Avishai Dernis 2025

using MIPS.Models.Addressing.Enums;
using System.IO;

namespace MIPS.Assembler.Models.Modules;

/// <summary>
/// A section in the <see cref="ModuleConstructor"/>.
/// </summary>
public class ModuleSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleSection"/> class.
    /// </summary>
    public ModuleSection(Section section, Stream? stream = null)
    {
        Section = section;
        Stream = stream ?? new MemoryStream();
    } 

    /// <summary>
    /// Gets the section type.
    /// </summary>
    public Section Section { get; }

    /// <summary>
    /// Gets the section stream data.
    /// </summary>
    public Stream Stream { get; }
}
