// Avishai Dernis 2025

using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Models.Instructions.Enums;
using System.IO;

namespace MIPS.Assembler.Models.Config;

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
    /// <param name="stream">The stream to write the module to. A new stream will be created if null.</param>
    /// <returns>The constructed module.</returns>
    public abstract IBuildModule? CreateModule(Module module, AssemblerConfig config, Stream? stream = null);
}
