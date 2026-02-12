// Avishai Dernis 2026

using Zarem.Emulator.Config;

namespace Zarem.Emulator;

/// <summary>
/// A base class for an emulator.
/// </summary>
/// <typeparam name="TConfig">The emulator configuration info</typeparam>
public abstract class Emulator<TConfig>
    where TConfig : EmulatorConfig

{
}
