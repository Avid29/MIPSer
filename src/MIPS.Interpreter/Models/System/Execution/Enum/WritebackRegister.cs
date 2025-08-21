﻿// Avishai Dernis 2025

namespace MIPS.Interpreter.Models.System.Execution.Enum;

/// <summary>
/// An enum describing which register to writeback to.
/// </summary>
public enum WritebackRegister
{
    #pragma warning disable CS1591
    
    None,
    GPR,
    CPR0,
    FPR,

    #pragma warning restore CS1591
}
