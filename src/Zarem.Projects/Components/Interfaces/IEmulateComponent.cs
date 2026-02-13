// Avishai Dernis 2026

using Zarem.Emulator.Config;

namespace Zarem.Components.Interfaces;

/// <summary>
/// An interface for a component of a <see cref="Project"/> that emulates machines.
/// </summary>
public interface IEmulateComponent : IProjectComponent
{
    /// <summary>
    /// Gets the emulator config.
    /// </summary>
    public EmulatorConfig Config { get; }
}
