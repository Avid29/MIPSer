// Adam Dernis 2025

using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace MIPS.Assembler.Models.Instructions.Abstract;

/// <summary>
/// A class for managing instruction lookup.
/// </summary>
public abstract class InstructionTableBase<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionTable"/> class.
    /// </summary>
    public InstructionTableBase(MipsVersion version)
    {
        Version = version;

        LookupTable = [];
        Initialize();
    }

    /// <summary>
    /// Gets the MIPS version of the instruction table.
    /// </summary>
    public MipsVersion Version { get; }

    /// <summary>
    /// The table of elements in the instruction table.
    /// </summary>
    protected Dictionary<T, InstructionMetadata> LookupTable { get; }
    
    /// <summary>
    /// Attempts to get an instruction by name.
    /// </summary>
    /// <param name="key">The key to lookup the instruction.</param>
    /// <param name="metadata">The instruction metadata.</param>
    /// <param name="requiredVersion">The required version to have this instruction, if there is one.</param>
    /// <returns>Whether or not an instruction exists by that name</returns>
    public virtual bool TryGetInstruction(T key, out InstructionMetadata metadata, out MipsVersion? requiredVersion)
    {
        requiredVersion = null;
        if (LookupTable.TryGetValue(key, out metadata))
            return true;

        return false;
    }

    /// <summary>
    /// Gets all instructions in the instruction table.
    /// </summary>
    /// <returns>An array of the instructions in the table.</returns>
    public InstructionMetadata[] GetInstructions() => [..LookupTable.Values];

    private void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();
        resources = [..resources.Where(x => x.EndsWith("Instructions.json"))];

        foreach (var resource in resources)
        {
            var instructions = LoadInstructionSet(assembly, resource);

            foreach (var instruction in instructions)
            {
                LoadInsturction(instruction);
            }
        }
    }

    /// <summary>
    /// Loads an instruction into the <see cref="LookupTable"/>.
    /// </summary>
    /// <param name="metadata">The metadata of the instruction.</param>
    protected abstract void LoadInsturction(InstructionMetadata metadata);

    private static InstructionMetadata[] LoadInstructionSet(Assembly assembly, string resourceName)
    {
        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return [];

        var instructions = JsonSerializer.Deserialize<InstructionMetadata[]>(stream);
        if (instructions is null)
            return [];

        return instructions;
    }
}
