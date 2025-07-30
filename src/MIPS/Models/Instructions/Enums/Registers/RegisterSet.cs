// Avishai Dernis 2025

namespace MIPS.Models.Instructions.Enums.Registers;

/// <summary>
/// An enum for register sets.
/// </summary>
public enum RegisterSet
{
    #pragma warning disable CS1591
    
    Numbered,
    GeneralPurpose,
    CoProc0,
    FloatingPoints,

    #pragma warning restore CS1591
}
