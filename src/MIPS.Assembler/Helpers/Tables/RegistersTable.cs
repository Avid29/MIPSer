// Adam Dernis 2024

using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Models.Instructions.Enums.Registers;
using System.Collections.Generic;
using System.Linq;

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
    /// <param name="set">Which register set table to reference.</param>
    /// <param name="logger">Logger </param>
    /// <returns>Whether or not an register exists by that name.</returns>
    public static bool TryGetRegister(string name, out Register register, out RegisterSet set, ILogger? logger = null)
    {
        register = Register.Zero;
        set = RegisterSet.Numbered;

        // Check for float register
        if (name[0] == 'f' && byte.TryParse(name[1..], out _))
        {
            // Fall-through to numerical register with 'f' removed..
            set = RegisterSet.FloatingPoints;
            name = name[1..];
        }

        // Check for numberical register
        if (byte.TryParse(name[0..], out var num))
        {
            // Lowest register enum to highest register enum
            if (num is < (byte)Register.Zero or > (byte)Register.ReturnAddress)
            {
                logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"No register of number {num} exists");
                return false;
            }

            // Cast num to register
            register = (Register)num;
            return true;
        }

        if (_gpRegisterTable.TryGetValue(name, out register))
        {
            set = RegisterSet.GeneralPurpose;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to get a register's string by value.
    /// </summary>
    /// <param name="register">The register value.</param>
    /// <param name="set">The set the register belongs to.</param>
    /// <returns>The name of the register as a string.</returns>
    public static string GetRegisterString(Register register, RegisterSet? set)
    {
        // Default to numbered.
        set ??= RegisterSet.Numbered;

        string name = set switch
        {
            // This is O(n) when it could easily be O(1), but n is 32. So idc.
            RegisterSet.GeneralPurpose => _gpRegisterTable.First(x => x.Value == register).Key,
            RegisterSet.FloatingPoints => $"f{(int)register}",
            //RegisterSet.CoProc0 => throw new System.NotImplementedException("CoProc0 registers not implemented."),    
            _ or RegisterSet.Numbered => $"{(int)register}",
        };

        // Prepend '$'
        return $"${name}";
    }

    private static readonly Dictionary<string, Register> _gpRegisterTable = new()
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
