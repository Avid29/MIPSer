// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

/// <summary>
/// A class containing a constant table for instruction lookup.
/// </summary>
public static partial class InstructionsTable
{
    private static Dictionary<string, InstructionMetadata>? _instructionTable;

    /// <summary>
    /// Attempts to get an instruction by name.
    /// </summary>
    /// <param name="name">The name of the instruction.</param>
    /// <param name="metadata">The instruction metadata.</param>
    /// <returns>Whether or not an instruction exists by that name</returns>
    public static bool TryGetInstruction(string name, out InstructionMetadata metadata)
        => InstructionTable.TryGetValue(name, out metadata);

    private static Dictionary<string, InstructionMetadata> InstructionTable => _instructionTable ?? InitFullTable();

    // This is terrible design, but it's relatively readable.
    // Fix later
    private static Dictionary<string, InstructionMetadata> InitFullTable()
    {
        _instructionTable = [];
        MergeDictionary(_rTypeInstructionTable);
        MergeDictionary(_iTypeInstructionTable);
        MergeDictionary(_jTypeInstructionTable);
        MergeDictionary(_regImmInstructionTable);
        MergeDictionary(_coProcInstructionTable);
        MergeDictionary(_pseudoInstructionTable);
        return _instructionTable;
    }

    private static void MergeDictionary(Dictionary<string, InstructionMetadata> dict)
    {
        foreach ((var key, var value) in dict)
        {
            _instructionTable?.Add(key, value);
        }
    }
}
