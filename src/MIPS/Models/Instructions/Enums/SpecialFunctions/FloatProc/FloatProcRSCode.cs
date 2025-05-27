// Adam Dernis 2025

using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Models.Instructions.Enums.SpecialFunctions.FloatProc;

/// <summary>
/// An enum for <see cref="OperationCode.Coprocessor1"/> instruction rs field function codes.
/// </summary>
public enum FloatProcRSCode
{
    /// <summary>
    /// Marks that there is no RS code.
    /// </summary>
    /// <remarks>
    /// This value is too large to encode in a real instruction. If by accident
    /// this were encoded into an <see cref="Instruction"/> struct, it would become 
    /// <see cref="MFC1"/> (or <see cref="Register.Zero"/>) upon unencoding.
    /// </remarks>
    None = 0x20,
    
    #pragma warning disable CS1591

    MFC1 = 0x00,
    CFC1 = 0x02,
    MFHC1 = 0x03,
    MTC1 = 0x04,
    CTC1 = 0x06,
    MTHC1 = 0x07,

    BC1 = 0x08,
    BC1ANY2 = 0x09,
    BC1EQZ = 0x09,
    BC1ANY4 = 0x0a,
    BZ_V = 0x0b,
    BC1NEZ = 0x0d,
    BNZ_V = 0x0f,

    Single = 0x10,
    Double = 0x11,
    Word = 0x14,
    Long = 0x15,
    PairedSingle = 0x16,

    BZ_B = 0x18,
    BZ_H = 0x19,
    BZ_W = 0x1a,
    BZ_D = 0x1b,
    BNZ_B = 0x1c,
    BNZ_H = 0x1d,
    BNZ_W = 0x1e,
    BNZ_D = 0x1f,

    #pragma warning restore CS1591
}
