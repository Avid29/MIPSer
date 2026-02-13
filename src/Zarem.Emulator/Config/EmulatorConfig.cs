// Avishai Dernis 2026

using System.Xml.Serialization;

namespace Zarem.Emulator.Config;

/// <summary>
/// A class containing emulator configurations.
/// </summary>
public class EmulatorConfig
{
    /// <summary>
    /// Gets or sets whether or not the host should handle traps.
    /// </summary>
    /// <remarks>
    /// If set to true, ALL traps will be handled by the host-layer, and not the emulated machine.
    /// This allows user-code programs to be interpreted by the emulator without loading and executing an OS.
    /// </remarks>
    public bool HostedTraps { get; set; }
}
