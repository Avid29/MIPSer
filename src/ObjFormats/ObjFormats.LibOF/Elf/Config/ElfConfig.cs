// Avishai Dernis 2025

using Zarem.Config;

namespace ObjectFiles.Elf.Config;

/// <summary>
/// A class containing elf configuration info.
/// </summary>
public class ElfConfig : FormatConfig
{
    /// <summary>
    /// Gets whether or not to use little endian.
    /// </summary>
    public bool IsLittleEndian { get; set; } = false;
}
