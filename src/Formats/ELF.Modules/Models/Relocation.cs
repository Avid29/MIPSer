// Adam Dernis 2025

using System.Numerics;

namespace ELF.Modules.Models;

/// <summary>
/// A struct containging info for a relocation entry without an appendation in the ELF format.
/// </summary>
public struct Relocation<TAddress>
    where TAddress : unmanaged, IBinaryInteger<TAddress>, IUnsignedNumber<TAddress>
{
    private TAddress _offset;
    private TAddress _info;
    
    /// <summary>
    /// Gets the offset associated with the entry.
    /// </summary>
    public TAddress Offset
    {
        readonly get => _offset;
        internal set => _offset = value;
    }
    
    /// <summary>
    /// Gets the additional information associated with the entry.
    /// </summary>
    public TAddress Info
    {
        readonly get => _info;
        internal set => _info = value;
    }
}
