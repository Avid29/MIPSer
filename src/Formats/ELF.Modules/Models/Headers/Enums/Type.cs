// Adam Dernis 2025

namespace ELF.Modules.Models.Headers.Enums;

/// <summary>
/// An enum for the type field of the ELF header format.
/// </summary>
public enum Type : ushort
{
    #pragma warning disable CS1591

    None,
    Relocatable,
    Executable,
    SharedObject,
    Core,

    #pragma warning restore
    
}
