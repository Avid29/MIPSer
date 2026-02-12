// Avishai Dernis 2026

using Zarem.Assembler.Config;
using Zarem.Config;
using Zarem.Emulator.Config;

namespace Zarem.MIPS.Projects;

/// <summary>
/// An <see cref="ProjectConfig{TAsmConfig, TEmuConfig}"/> implementation for MIPS.
/// </summary>
public sealed class MIPSProjectConfig : ProjectConfig<MIPSAssemblerConfig, MIPSEmulatorConfig>
{
}
