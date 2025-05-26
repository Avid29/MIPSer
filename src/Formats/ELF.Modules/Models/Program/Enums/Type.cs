// Adam Dernis 2025

namespace ELF.Modules.Models.Program.Enums;

/// <summary>
/// An enum for the type field of the ELF program header format.
/// </summary>
public enum Type : uint
{
    #pragma warning disable CS1591

    Load = 0x1,
    Dynamic,
    Interpreter,
    Auxiliary,
    SharedLibrary,
    ProgramHeader,
    ThreadLocalStorage,

    #pragma warning restore CS1591
}
