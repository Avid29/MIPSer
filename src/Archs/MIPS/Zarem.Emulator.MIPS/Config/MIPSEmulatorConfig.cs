// Avishai Dernis 2026

using System.Xml.Serialization;
using Zarem.Models.Instructions.Enums;

namespace Zarem.Emulator.Config;

/// <summary>
/// A class containing emulator configurations for the MIPS emulator.
/// </summary>
public class MIPSEmulatorConfig : EmulatorConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSEmulatorConfig"/> class.
    /// </summary>
    public MIPSEmulatorConfig()
    {
        MipsVersion = MipsVersion.MipsIV;
    }

    /// <summary>
    /// Gets or sets the mips ISA version to emulate.
    /// </summary>
    [XmlElement]
    public MipsVersion MipsVersion { get; set; }
}
