// Adam Dernis 2025

using ELF.Modules.Models.Headers;
using ELF.Modules.Models.Headers.Enums;
using MIPS.Assembler.Models.Config;
using MIPS.Models.Instructions.Enums;

namespace ELF.Modules.Config;

/// <summary>
/// A class containing ELF configuration info.
/// </summary>
public class ElfConfig : AssemblerConfig
{
    private const uint MAGIC = 0x7F45_4C46; // 0x7C ELF
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ElfConfig"/> class.
    /// </summary>
    public ElfConfig(MipsVersion version = MipsVersion.MipsII) : base(version)
    {
    }

    /// <summary>
    /// Gets or sets the header identify info.
    /// </summary>
    public HeaderIdentity ElfIdentity { get; set; } = new()
    {
        Magic = MAGIC,
        Class = Class.Bit32,
        Data = Data.BigEndian,
        Version = 1,
    };
}
