// Adam Dernis 2024

using MIPS.Models.Instructions.Enums;

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
    /// Gets or sets whether or not the assembler should allow pseudo instructions.
    /// </summary>
    public MipsVersion MipsVersion { get; init; }

    /// <summary>
    /// Gets or sets whether or not the assembler should allow pseudo instructions.
    /// </summary>
    public bool AllowPseudos { get; init; } = true;

    /// <summary>
    /// Gets the default configuration.
    /// </summary>
    public static AssemblerConfig Default => new();
}
