// Avishai Dernis 2026

using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Attributes;
using Zarem.Config;
using Zarem.Emulator;
using Zarem.Emulator.Config;
using Zarem.Models.Instructions.Enums;

namespace Zarem.MIPS;

/// <summary>
/// An <see cref="ProjectConfig{TAsmConfig, TEmuConfig}"/> implementation for MIPS.
/// </summary>
[ProjectType("MIPS", typeof(MIPSAssembler), typeof(MIPSEmulator))]
public sealed class MIPSProjectConfig : ProjectConfig<MIPSAssemblerConfig, MIPSEmulatorConfig>
{
    /// <summary>
    /// Gets the mips version 
    /// </summary>
    public MipsVersion MipsVersion
    {
        get => field;
        set
        {
            field = value;

            AssemblerConfig?.MipsVersion = value;
            EmulatorConfig?.MipsVersion = value;
        }
    }
}
