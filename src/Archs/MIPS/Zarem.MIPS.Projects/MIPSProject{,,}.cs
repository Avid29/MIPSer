// Avishai Dernis 2026

using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Assembler.Models.Modules;
using Zarem.Attributes;
using Zarem.Config;
using Zarem.Emulator;
using Zarem.Emulator.Config;
using Zarem.Emulator.Models.Modules;

namespace Zarem.MIPS.Projects;

/// <summary>
/// A <see cref="Project{TAssembler, TEmulator, TModule, TAsmConfig, TEmuConfig, TModConfig}"/> implementation for mips.
/// </summary>
[ProjectType("MIPS", typeof(MIPSProjectConfig))]
public sealed class MIPSProject<TModule, TModConfig> : Project<MIPSAssembler, MIPSEmulator, TModule, MIPSAssemblerConfig, MIPSEmulatorConfig, TModConfig>
    where TModule : IExecutableModule, IBuildModule<TModule, TModConfig>
    where TModConfig : FormatConfig, new()
    
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSProject{TModule, TModConfig}"/> class.
    /// </summary>
    public MIPSProject(MIPSProjectConfig config) : base(config)
    {
    }
}
