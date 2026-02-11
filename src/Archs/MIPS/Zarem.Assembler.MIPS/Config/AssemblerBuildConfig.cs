// Avishai Dernis 2025

using Zarem.Assembler.MIPS.Models.Modules;
using Zarem.Assembler.MIPS.Models.Modules.Interfaces;
using Zarem.MIPS.Models.Instructions.Enums;

namespace Zarem.Assembler.MIPS.Config;

/// <summary>
/// A base class for a build ready <see cref="AssemblerConfig"/>.
/// </summary>
public abstract class AssemblerBuildConfig : AssemblerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerBuildConfig"/> class.
    /// </summary>
    public AssemblerBuildConfig(MipsVersion version = MipsVersion.MipsII) : base(version)
    {
    }

    /// <summary>
    /// Creates a module from a <see cref="Module"/>.
    /// </summary>
    /// <param name="module">The <see cref="Module"/> to build from.</param>
    /// <param name="config">The configuration settings.</param>
    /// <returns>The constructed module.</returns>
    public abstract IBuildModule? CreateModule(Module module, AssemblerConfig config);
}
