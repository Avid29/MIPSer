// Adam Dernis 2024

using MIPS.Models.Addressing;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Modules.Tables.Enums;
using System.Diagnostics.CodeAnalysis;

namespace MIPS.Models.Modules.Tables;

/// <summary>
/// An entry in the load module's symbol table.
/// </summary>
public struct SymbolEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolEntry"/> class.
    /// </summary>
    public SymbolEntry(string name, SymbolType type, Address address, SymbolFlags flags = 0)
    {
        Name = name;
        Type = type;
        Address = address;
        Flags = flags;
    }

    /// <summary>
    /// Gets or sets the symbol name of the entry.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value of the symbol as an <see cref="Addressing.Address"/>.
    /// </summary>
    public Address Address { get; set; }

    /// <summary>
    /// Gets or sets the symbol type.
    /// </summary>
    public SymbolType Type { get; set; }

    /// <summary>
    /// Gets or sets the symbol flag info.
    /// </summary>
    public SymbolFlags Flags { get; set; }

    /// <summary>
    /// Gets or sets if the symbol is global.
    /// </summary>
    public bool Global
    {
        get => CheckFlag(SymbolFlags.Global);
        set => SetFlags(SymbolFlags.Global, value);
    }

    /// <summary>
    /// Gets if the symbol is forward defined.
    /// </summary>
    public bool ForwardDefined 
    {
        get => CheckFlag(SymbolFlags.ForwardDefined);
        set => SetFlags(SymbolFlags.ForwardDefined, value);
    }

    /// <summary>
    /// Gets whether or not the symbol is defined.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Address))]
    public readonly bool IsDefined => Address.Section is not Section.External;

    /// <summary>
    /// Gets if a flag is set on the symbol entry.
    /// </summary>
    /// <param name="flag">The flag to check.</param>
    /// <returns>True if the flag is set, false otherwise.</returns>
    private bool CheckFlag(SymbolFlags flag) => Flags.HasFlag(flag);

    /// <summary>
    /// Sets a set of flags on the symbol entry.
    /// </summary>
    /// <param name="flags">The flags to set.</param>
    /// <param name="state">The new state of the flag.</param>
    private void SetFlags(SymbolFlags flags, bool state = true)
    {
        // Clear the flag.
        Flags &= ~flags;

        // Set the flag to true.
        if (state)
        {
            Flags |= flags;
        }
    }
}
