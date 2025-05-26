// Adam Dernis 2025

namespace ELF.Modules.Models.Headers.Enums;

/// <summary>
/// An enum for the type field of the ELF program header format.
/// </summary>
public enum ProgramType : uint
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
