// Adam Dernis 2024

using System.Xml.Serialization;
using Zarem.Config;

namespace ObjFormats.RASM.Config;

/// <summary>
/// A class containing rasm configuration info.
/// </summary>
public class RasmConfig : FormatConfig
{
    private const ushort MAGIC = 0xFA_CE;
    private const ushort VERSION = 0x2C_C6;

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
