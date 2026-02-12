// Avishai Dernis 2026

using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Attributes;
using Zarem.Config;
using Zarem.Emulator;
using Zarem.Emulator.Config;

namespace Zarem.MIPS.Projects;

/// <summary>
/// A <see cref="Project{TAssembler, TEmulator, TAsmConfig, TEmuConfig}"/> implementation for mips.
/// </summary>
[ProjectType("MIPS", typeof(MIPSProjectConfig))]
public sealed class MIPSProject : Project<MIPSAssembler, MIPSEmulator, MIPSAssemblerConfig, MIPSEmulatorConfig>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSProject"/> class.
    /// </summary>
    public MIPSProject(MIPSProjectConfig config) : base(config)
    {
    }
}
