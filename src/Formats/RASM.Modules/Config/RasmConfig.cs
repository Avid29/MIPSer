// Adam Dernis 2024

using MIPS.Assembler.Models;
using MIPS.Models.Instructions.Enums;

namespace RASM.Modules.Config;

/// <summary>
/// A class containing rasm configuration info.
/// </summary>
public class RasmConfig : AssemblerConfig
{
    private const ushort MAGIC = 0xFA_CE;
    private const ushort VERSION = 0x2C_C6;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerConfig"/> class.
    /// </summary>
    public RasmConfig(MipsVersion version = MipsVersion.MipsII) : base(version)
    {
    }

    /// <summary>
    /// Gets or sets the header magic value.
    /// </summary>
    public ushort MagicNumber { get; set; } = MAGIC;

    /// <summary>
    /// Gets or sets the header version value.
    /// </summary>
    public ushort VersionNumber { get; set; } = VERSION;
}
