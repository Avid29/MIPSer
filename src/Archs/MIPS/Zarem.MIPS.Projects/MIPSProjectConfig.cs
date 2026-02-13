// Avishai Dernis 2026

using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Attributes;
using Zarem.Config;
using Zarem.Emulator;
using Zarem.Emulator.Config;

namespace Zarem.MIPS.Projects;

/// <summary>
/// An <see cref="ProjectConfig{TAsmConfig, TEmuConfig}"/> implementation for MIPS.
/// </summary>
[ProjectType("MIPS", typeof(MIPSAssembler), typeof(MIPSEmulator))]
public sealed class MIPSProjectConfig : ProjectConfig<MIPSAssemblerConfig, MIPSEmulatorConfig>
{
}
