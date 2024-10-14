// Adam Dernis 2024

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for pesudo-instruction types.
/// </summary>
public enum PseudoOp
{
    #pragma warning disable CS1591

    NoOperation,
    SuperScalarNoOperation,
    BranchAndLink,
    BranchOnLessThan,
    LoadImmediate,
    AbsoluteValue,
    Move,
    LoadAddress,
    SetGreaterThanOrEqual,
        
    #pragma warning restore CS1591
}
