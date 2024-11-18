// Adam Dernis 2024

using MIPS.Models.Instructions.Enums.Registers;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

/// <summary>
/// A class containing a constant table for register lookup.
/// </summary>
public static class RegistersTable
{
    /// <summary>
    /// Attempts to get a register by name.
    /// </summary>
    /// <param name="name">The name of the register.</param>
    /// <param name="register">The register enum value.</param>
    /// <returns>Whether or not an register exists by that name</returns>
    public static bool TryGetRegister(string name, out Register register)
        => _registerTable.TryGetValue(name, out register);

    private static readonly Dictionary<string, Register> _registerTable = new()
    {
        { "zero", Register.Zero },

        { "at", Register.AssemblerTemporary },

        { "v0", Register.ReturnValue0 },
        { "v1", Register.ReturnValue1 },

        { "a0", Register.Argument0 },
        { "a1", Register.Argument1 },
        { "a2", Register.Argument2 },
        { "a3", Register.Argument3 },

        { "t0", Register.Temporary0 },
        { "t1", Register.Temporary1 },
        { "t2", Register.Temporary2 },
        { "t3", Register.Temporary3 },
        { "t4", Register.Temporary4 },
        { "t5", Register.Temporary5 },
        { "t6", Register.Temporary6 },
        { "t7", Register.Temporary7 },

        { "s0", Register.Saved0 },
        { "s1", Register.Saved1 },
        { "s2", Register.Saved2 },
        { "s3", Register.Saved3 },
        { "s4", Register.Saved4 },
        { "s5", Register.Saved5 },
        { "s6", Register.Saved6 },
        { "s7", Register.Saved7 },

        { "t8", Register.Temporary8 },
        { "t9", Register.Temporary9 },

        { "k0", Register.Kernel0 },
        { "k1", Register.Kernel1 },

        { "gp", Register.GlobalPointer },
        { "sp", Register.StackPointer },
        { "fp", Register.FramePointer },

        { "ra", Register.ReturnAddress },
    };
}
