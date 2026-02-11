// Adam Dernis 2025

using Zarem.MIPS.Services.Interfaces;

namespace Zarem.MIPS.Services;

/// <summary>
/// A collection of global services.
/// </summary>
public static class ServiceCollection
{
    #if DEBUG

    /// <summary>
    /// Gets the <see cref="DisassemblerService"/>.
    /// </summary>
    public static IDisassemblerService? DisassemblerService { get; set; }

    #endif
}
