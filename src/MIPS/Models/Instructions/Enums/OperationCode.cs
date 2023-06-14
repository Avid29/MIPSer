// Adam Dernis 2023

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for instruction op codes.
/// </summary>
public enum OperationCode : byte
{
    /// <summary>
    /// Marks any r-type instruction, each one shares an op-code of 0x00.
    /// </summary>
    /// <remarks>
    /// r-type instructions are distinguished with <see cref="FunctionCode"/>.
    /// </remarks>
    RType = 0x00,
    
    /// <summary>
    /// Marks a conditional branch instruction. See <see cref="BranchCode"/>.
    /// </summary>
    BranchConditional = 0x01,

    #pragma warning disable CS1591

    Jump = 0x02,
    JumpAndLink = 0x03,
    
    BranchOnEquals = 0x04,
    BranchOnNotEquals = 0x05,
    BranchOnLessThanOrEqualToZero = 0x06,
    BranchGreaterThanZero = 0x07,

    AddImmediate = 0x08,
    AddImmediateUnsigned = 0x09,

    SetLessThanImmediate = 0x0a,
    SetLessThanImmediateUnsigned = 0x0b,

    AndImmediate = 0x0c,
    OrImmediate = 0x0d,
    ExclusiveOrImmediate = 0x0e,

    LoadUpperImmediate = 0x0f,

    Coprocessor0 = 0x10,
    Coprocessor1 = 0x11,
    Coprocessor2 = 0x12,
    Coprocessor3 = 0x13,

    LoadByte = 0x20,
    LoadHalfWord = 0x21,
    LoadWordLeft = 0x22,
    LoadWord = 0x23,
    LoadByteUnsigned = 0x24,
    LoadHalfWordUnsigned = 0x25,
    LoadWordRight = 0x26,

    StoreByte = 0x28,
    StoreHalfWord = 0x29,
    StoreWordLeft = 0x2a,
    StoreWord = 0x2b,
    StoreWordRight = 0x2e,

    LoadWordCoprocessor0 = 0x30,
    LoadWordCoprocessor1 = 0x31,
    LoadWordCoprocessor2 = 0x32,
    LoadWordCoprocessor3 = 0x33,

    StoreWordCoprocessor0 = 0x38,
    StoreWordCoprocessor1 = 0x39,
    StoreWordCoprocessor2 = 0x3a,
    StoreWordCoprocessor3 = 0x3b,

#pragma warning restore CS1591
}
