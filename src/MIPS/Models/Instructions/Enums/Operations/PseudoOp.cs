﻿// Adam Dernis 2025

namespace MIPS.Models.Instructions.Enums.Operations;

/// <summary>
/// An enum for pesudo-instruction types.
/// </summary>
public enum PseudoOp
{
    #pragma warning disable CS1591

    NoOperation,
    SuperScalarNoOperation,
    UnconditionalBranch,
    BranchAndLink,
    BranchOnLessThan,
    LoadImmediate,
    AbsoluteValue,
    Move,
    LoadAddress,
    SetGreaterThanOrEqual,
        
    #pragma warning restore CS1591
}
