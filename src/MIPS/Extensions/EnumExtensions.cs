// Adam Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Extensions;

/// <summary>
/// A class containing extension methods for MIPS enums.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the <see cref="Argument"/> as a usage pattern string.
    /// </summary>
    public static string GetArgPatternString(this Argument argument)
        => argument switch
        {
            Argument.RS => "$rs",
            Argument.RT => "$rt",
            Argument.RD => "$rd",
            Argument.Shift => "shift",
            Argument.Immediate => "immediate",
            Argument.Offset => "offset",
            Argument.Address => "addr",
            Argument.AddressBase => "offset(base)",
            Argument.FullImmediate => "immediate",
            Argument.FS => "$fs",
            Argument.FT => "$ft",
            Argument.FD => "$fd",
            _ => ThrowHelper.ThrowArgumentException<string>($"{argument} out of range.")
        };
}
