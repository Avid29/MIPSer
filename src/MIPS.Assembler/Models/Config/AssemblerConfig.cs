// Adam Dernis 2024

using MIPS.Assembler.Models.Enums;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MIPS.Assembler.Models.Config;

/// <summary>
/// A class containing assembler configurations.
/// </summary>
public class AssemblerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerConfig"/> class.
    /// </summary>
    public AssemblerConfig() : this(MipsVersion.MipsII)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerConfig"/> class.
    /// </summary>
    public AssemblerConfig(MipsVersion version)
    {
        MipsVersion = version;
    }

    /// <summary>
    /// Gets or sets the mips version to use.
    /// </summary>
    [XmlElement]
    public MipsVersion MipsVersion { get; set; }

    /// <summary>
    /// Gets whether the <see cref="PseudoInstructionSet"/> is a blacklist or whitelist.
    /// </summary>
    [XmlElement]
    public PseudoInstructionPermissibility? PseudoInstructionPermissibility { get; set; }

    /// <summary>
    /// Gets the set of pseudo instructions to use as either a black or white list.
    /// </summary>
    [XmlElement]
    public HashSet<string>? PseudoInstructionSet { get; set; } = [];

    /// <summary>
    /// Gets the threshold alignment size where the assembler will give a message.
    /// </summary>
    [XmlElement]
    public int? AlignMessageThreshold { get; set; }

    /// <summary>
    /// Gets the threshold alignment size where the assembler will give a warning.
    /// </summary>
    [XmlElement]
    public int? AlignWarningThreshold { get; set; }

    /// <summary>
    /// Gets the threshold alignment size where the assembler will give a warning.
    /// </summary>
    [XmlElement]
    public int? SpaceMessageThreshold { get; set; }
}
