// Adam Dernis 2023

using MIPS.Assembler.Models.Directives.Abstract;

namespace MIPS.Assembler.Models.Directives;

/// <summary>
/// A <see cref="Directive"/> for data allocations.
/// </summary>
public class DataDirective : Directive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataDirective"/> class.
    /// </summary>
    public DataDirective(byte[] data)
    {
        Data = data;
    }

    /// <summary>
    /// Gets the data allocated by the directive.
    /// </summary>
    public byte[] Data { get; }
}
