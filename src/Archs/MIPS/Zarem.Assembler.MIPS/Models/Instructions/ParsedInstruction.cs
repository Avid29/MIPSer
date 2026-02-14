// Adam Dernis 2024

using System.Diagnostics.CodeAnalysis;
using Zarem.Assembler.Parsers;
using Zarem.Models.Instructions;
using Zarem.Models.Modules.Tables;

namespace Zarem.Assembler.Models.Instructions;

/// <summary>
/// An instruction as parsed by the <see cref="InstructionParser"/>.
/// </summary>
public class ParsedInstruction
{
    private readonly MIPSInstruction? _real;
    private readonly PseudoInstruction? _pseudo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedInstruction"/> class.
    /// </summary>
    public ParsedInstruction(MIPSInstruction instruction, ReferenceEntry? reference = null)
    {
        _real = instruction;
        Reference = reference;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedInstruction"/> class.
    /// </summary>
    public ParsedInstruction(PseudoInstruction instruction, ReferenceEntry? reference = null)
    {
        _pseudo = instruction;
        Reference = reference;
    }

    /// <summary>
    /// Gets the symbol referenced, or null if none.
    /// </summary>
    public ReferenceEntry? Reference { get; }

    /// <summary>
    /// Gets whether or not the parsed instruction was a pseudo instruction.
    /// </summary>
    [MemberNotNullWhen(false, nameof(_real))]
    [MemberNotNullWhen(true, nameof(_pseudo))]
    public bool IsPseduoInstruction => _real is null;

    /// <summary>
    /// Gets the parsed instruction implemented exlcusively in real instructions.
    /// </summary>
    public MIPSInstruction[] Realize()
    {
        if (!IsPseduoInstruction)
            return [_real.Value];

        return _pseudo.Value.Expand();
    }
}
