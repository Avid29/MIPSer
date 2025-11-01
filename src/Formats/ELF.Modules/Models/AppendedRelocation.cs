// Adam Dernis 2025

using ELF.Modules.Models.Enums;
using System.Numerics;

namespace ELF.Modules.Models;

/// <summary>
/// A struct containging info for a relocation entry with an appendation in the ELF format.
/// </summary>
public struct AppendedRelocation<TAddress>
    where TAddress : unmanaged, IBinaryInteger<TAddress>, IUnsignedNumber<TAddress>
{
    private TAddress _offset;
    private TAddress _info;
    private TAddress _append;

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
    
    /// <summary>
    /// Gets the appended data or reference associated with the entry.
    /// </summary>
    public TAddress Append
    {
        readonly get => _append;
        internal set => _append = value;
    }

    /// <summary>
    /// Gets the type of relocation.
    /// </summary>
    public readonly RelocationTypes Type => (RelocationTypes)(byte.CreateTruncating(Info));
}
