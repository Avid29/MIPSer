// Adam Dernis 2024

using MIPS.Assembler.Config;
using MIPS.Models.Instructions.Enums;
using System.Xml.Serialization;

namespace RASM.Modules.Config;

/// <summary>
/// A class containing rasm configuration info.
/// </summary>
public class RasmConfig : AssemblerBuildConfig<RasmConfig, RasmModule>
{
    private const ushort MAGIC = 0xFA_CE;
    private const ushort VERSION = 0x2C_C6;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RasmConfig"/> class.
    /// </summary>
    public RasmConfig() : this(MipsVersion.MipsIII)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RasmConfig"/> class.
    /// </summary>
    public RasmConfig(MipsVersion version) : base(version)
    {
    }

    /// <summary>
    /// Gets or sets the header magic value.
    /// </summary>
    [XmlIgnore]
    public ushort MagicNumber { get; set; } = MAGIC;

    /// <summary>
    /// Gets or sets the header version value.
    /// </summary>
    [XmlElement("RasmVersion")]
    public ushort VersionNumber { get; set; } = VERSION;
}
