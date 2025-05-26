// Adam Dernis 2025

namespace EFL.Modules.Models.Header.Enums;

/// <summary>
/// An enum for the type field of the ELF header format.
/// </summary>
public enum Type : short
{
    #pragma warning disable CS1591

    None,
    Relocatable,
    Executable,
    SharedObject,
    Core,

    #pragma warning restore
    
}
