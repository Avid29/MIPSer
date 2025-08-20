// Avishai Dernis 2025

using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using System;
using System.Linq;

namespace MIPS.Assembler.Helpers.Tables;

/// <summary>
/// A class containing a constant table for argument lookup.
/// </summary>
public static class ArgumentTable
{
    /// <summary>
    /// Gets the <see cref="Argument"/> as a usage pattern string.
    /// </summary>
    public static string GetArgPatternString(Argument argument) => _argumentTable[(int)argument];

    /// <summary>
    /// Attempts to get an argument by name.
    /// </summary>
    /// <param name="name">The name of the argument.</param>
    /// <param name="argument">The argument enum value.</param>
    /// <returns>Whether or not an argument exists by that name.</returns>
    public static bool TryGetArgument(string name, out Argument argument)
    {
        name = name.Trim();
        argument = (Argument)Array.IndexOf(_argumentTable, name);
        return argument is not (Argument)(-1);
    }

    private static string[] _argumentTable =
    {
        "$rs",
        "$rt",
        "$rd",
        "shift",
        "immediate",
        "offset",
        "address",
        "offset(base)",
        "immediate",
        "$fs",
        "$ft",
        "$fd"
    };
}
