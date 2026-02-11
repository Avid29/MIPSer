// Avishai Dernis 2026

using Zarem.MIPS.Models.Instructions.Enums;
using System.Xml.Serialization;

namespace Zarem.Emulator.MIPS.Config;

/// <summary>
/// A class containing emulator configurations.
/// </summary>
public class EmulatorConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmulatorConfig"/> class.
    /// </summary>
    public EmulatorConfig(MipsVersion version = MipsVersion.MipsIII)
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
