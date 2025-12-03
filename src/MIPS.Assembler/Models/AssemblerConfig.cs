// Adam Dernis 2024

using MIPS.Assembler.Models.Enums;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Models;

/// <summary>
/// A class containing assembler configurations.
/// </summary>
public class AssemblerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerConfig"/> class.
    /// </summary>
    public AssemblerConfig(MipsVersion version = MipsVersion.MipsII)
    {
        MipsVersion = version;
    }

    /// <summary>
    /// Gets whether or not the assembler should allow pseudo instructions.
    /// </summary>
    public MipsVersion MipsVersion { get; init; }

    /// <summary>
    /// Gets whether the <see cref="PseudoInstructionSet"/> is a blacklist or whitelist.
    /// </summary>
    public PseudoInstructionPermissibility PseudoInstructionPermissibility { get; init; } = PseudoInstructionPermissibility.Blacklist;

    /// <summary>
    /// Gets the set of pseudo instructions to use as either a black or white list.
    /// </summary>
    public HashSet<string> PseudoInstructionSet { get; init; } = [];

    /// <summary>
    /// Gets the threshold alignment size where the assembler will give a message.
    /// </summary>
    public int AlignMessageThreshold { get; init; } = 7;

    /// <summary>
    /// Gets the threshold alignment size where the assembler will give a warning.
    /// </summary>
    public int AlignWarningThreshold { get; init; } = 17;

    /// <summary>
    /// Gets the threshold alignment size where the assembler will give a warning.
    /// </summary>
    public int SpaceMessageThreshold { get; init; } = 4096;

    /// <summary>
    /// Gets the default configuration.
    /// </summary>
    public static AssemblerConfig Default => new();
}
