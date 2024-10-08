// Adam Dernis 2023

using MIPS.Assembler.Models.Directives.Abstract;

namespace MIPS.Assembler.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for data allocations.
/// </summary>
public class DataDirective(byte[] data) : Directive
{
    /// <summary>
    /// Gets the data allocated by the directive.
    /// </summary>
    public byte[] Data { get; } = data;
}
