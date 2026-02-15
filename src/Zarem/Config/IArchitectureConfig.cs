// Avishai Dernis 2026

using Zarem.Assembler.Config;
using Zarem.Emulator.Config;

namespace Zarem.Config;

/// <summary>
/// An interface for an architecture's configuration.
/// </summary>
public interface IArchitectureConfig
{
    /// <summary>
    /// Gets the assembler configuration.
    /// </summary>
    AssemblerConfig? AssemblerConfig { get; }

    /// <summary>
    /// Gets the emulator config.
    /// </summary>
    EmulatorConfig? EmulatorConfig { get; }
}
