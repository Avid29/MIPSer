// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text.Json;

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

    private static Dictionary<string, InstructionMetadata> InitFullTable()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();
        resources = resources.Where(x => x.EndsWith("Instructions.json")).ToArray();

        _instructionTable = [];

        foreach (var resource in resources)
        {
            var instructions = LoadInstructionSet(assembly, resource);

            foreach(var instruction in instructions)
            {
                _instructionTable.Add(instruction.Name, instruction);
            }
        }

        return _instructionTable;
    }

    private static InstructionMetadata[] LoadInstructionSet(Assembly assembly, string resourceName)
    {
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is null)
                return [];

            var instructions = JsonSerializer.Deserialize<InstructionMetadata[]>(stream);
            if (instructions is null)
                return [];

            return instructions;
        }
    }
}
