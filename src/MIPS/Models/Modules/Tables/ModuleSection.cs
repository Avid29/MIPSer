// Avishai Dernis 2025

using MIPS.Assembler.Models.Modules;

// Avishai Dernis 2025

using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// A section in the <see cref="Module"/>.
/// </summary>
public class ModuleSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleSection"/> class.
    /// </summary>
    public ModuleSection(string name, SectionFlags flags, Stream? stream = null)
    {
        Name = name;
        Flags = flags;
        Stream = stream ?? new MemoryStream();
    } 

    /// <summary>
    /// Gets the section's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the sections's flags.
    /// </summary>
    public SectionFlags Flags { get; }

    /// <summary>
    /// Gets the section stream data.
    /// </summary>
    public Stream Stream { get; }
}
