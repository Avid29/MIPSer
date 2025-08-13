﻿// Avishai Dernis 2025

namespace Mipser.Models.CheatSheet.Enums;

/// <summary>
/// An enum representing the purpose of an encoding pattern section.
/// </summary>
public enum EncodingSectionType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    Reserved = -1,
    OpCode = 0,
    FunctionCode,
    RegisterFuncCode,
    GeneralPurposeRegister,
    Immediate,
    CoProcessorRegister,
    Format,

    // Composite purposes
    GeneralOrCoProcessorRegister,


#pragma warning restore CS1591
}
