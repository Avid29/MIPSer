﻿// Adam Dernis 2024

using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace MIPS.Assembler.Models.Instructions;

/// <summary>
/// A class for managing instruction lookup by name.
/// </summary>
public class InstructionTable
{
    private MipsVersion _version;
    private readonly Dictionary<string, InstructionMetadata> _instructionTable;
    private readonly Dictionary<string, HashSet<MipsVersion>> _outOfVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionTable"/> class.
    /// </summary>
    public InstructionTable(MipsVersion version)
    {
        _version = version;

        _instructionTable = [];
        _outOfVersion = [];
        Initialize();
    }

    /// <summary>
    /// Attempts to get an instruction by name.
    /// </summary>
    /// <param name="name">The name of the instruction.</param>
    /// <param name="metadata">The instruction metadata.</param>
    /// <param name="requiredVersion">The required version to have this instruction, if there is one.</param>
    /// <returns>Whether or not an instruction exists by that name</returns>
    public bool TryGetInstruction(string name, out InstructionMetadata metadata, out MipsVersion? requiredVersion)
    {
        requiredVersion = null;
        if (_instructionTable.TryGetValue(name, out metadata))
            return true;
        
        if (_outOfVersion.TryGetValue(name, out var versions))
        {
            // Higher version instruction. Get the lowest version available.
            if (_version < versions.FirstOrDefault())
                requiredVersion = versions.Min();
            // Deprecated instruction. Get the highest version available.
            else
                requiredVersion = versions.Max();
        }

        return false;
    }

    internal InstructionMetadata[] GetInstructions() => [.._instructionTable.Values];

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

    private void LoadInsturction(InstructionMetadata metadata)
    {
        if (metadata.MIPSVersions.Contains(_version))
        {
            _instructionTable.Add(metadata.Name, metadata);
        }
        else
        {
            _outOfVersion.Add(metadata.Name, metadata.MIPSVersions);
        }
    }
    
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
