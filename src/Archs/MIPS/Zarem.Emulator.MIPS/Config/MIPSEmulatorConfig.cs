// Avishai Dernis 2026

using System.Xml.Serialization;
using Zarem.Models.Instructions.Enums;

namespace Zarem.Emulator.Config;

/// <summary>
/// A class containing emulator configurations.
/// </summary>
public class MIPSEmulatorConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSEmulatorConfig"/> class.
    /// </summary>
    public MIPSEmulatorConfig(MipsVersion version = MipsVersion.MipsIII)
    {
        MipsVersion = version;
    }

    /// <summary>
    /// Gets or sets the mips ISA version to emulate.
    /// </summary>
    [XmlElement]
    public MipsVersion MipsVersion { get; set; }

    /// <summary>
    /// Gets or sets whether or not the host should handle traps.
    /// </summary>
    /// <remarks>
    /// If set to true, ALL traps will be handled by the host-layer, and not the emulated machine.
    /// This allows user-code programs to be interpreted by the emulator without loading and executing an OS.
    /// </remarks>
    [XmlElement]
    public bool HostedTraps { get; set; }
}
