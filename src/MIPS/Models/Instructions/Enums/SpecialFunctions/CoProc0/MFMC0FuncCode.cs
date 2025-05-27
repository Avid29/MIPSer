﻿// Adam Dernis 2025

namespace MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;

/// <summary>
/// An enum for <see cref="CoProc0RSCode.MFMC0"/> instruction function codes.
/// </summary>
public enum MFMC0FuncCode
{
    #pragma warning disable CS1591

    DisableInterupts = 0x0,

    EnableVirtualProcessor = 0x4,

    EnableInterupts = 0x20,
    DisableVirtualProcessor = 0x24,

    #pragma warning restore CS1591
}
