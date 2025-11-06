// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions.Abstract;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MIPS.Assembler.Models.Instructions;

/// <summary>
/// A class for managing instruction lookup by name.
/// </summary>
public class InstructionTable : InstructionTableBase<string>
{
    private readonly Dictionary<string, HashSet<MipsVersion>> _outOfVersion = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionTable"/> class.
    /// </summary>
    public InstructionTable(MipsVersion version) : base(version)
    {
    }

    /// <summary>
    /// Attempts to get an instruction by name.
    /// </summary>
    /// <param name="name">The name of the instruction.</param>
    /// <param name="metadatas">The instruction metadatas.</param>
    /// <param name="requiredVersion">The required version to have this instruction, if there is one.</param>
    /// <returns>Whether or not an instruction exists by that name</returns>
    public override bool TryGetInstruction(string name, [NotNullWhen(true)] out List<InstructionMetadata>? metadatas, out MipsVersion? requiredVersion)
    {
        requiredVersion = null;
        if (base.TryGetInstruction(name, out metadatas, out _))
            return true;
        
        if (_outOfVersion.TryGetValue(name, out var versions))
        {
            // Higher version instruction. Get the lowest version available.
            if (Version < versions.FirstOrDefault())
                requiredVersion = versions.Min();
            // Deprecated instruction. Get the highest version available.
            else
                requiredVersion = versions.Max();
        }

        return false;
    }

    /// <summary>
    /// Attempts to get an instruction by name.
    /// </summary>
    /// <param name="name">The name of the instruction.</param>
    /// <param name="argCount">The number of arguments for the instruction.</param>
    /// <param name="metadata">The instruction metadatas.</param>
    /// <param name="requiredVersion">The required version to have this instruction, if there is one.</param>
    /// <returns>Whether or not an instruction exists by that name</returns>
    public bool TryGetInstruction(string name, int argCount, out InstructionMetadata metadata, out MipsVersion? requiredVersion)
    {
        metadata = default;
        requiredVersion = null;
        if (base.TryGetInstruction(name, out var metadatas, out _))
        {
            if (metadatas is null)
                return false;

            if (!metadatas.Any(x => x.ArgumentPattern.Length != argCount))
                return false;

            metadata = metadatas.FirstOrDefault(x => x.ArgumentPattern.Length == argCount);
            return true;
        }
        
        if (_outOfVersion.TryGetValue(name, out var versions))
        {
            // Higher version instruction. Get the lowest version available.
            if (Version < versions.FirstOrDefault())
                requiredVersion = versions.Min();
            // Deprecated instruction. Get the highest version available.
            else
                requiredVersion = versions.Max();
        }

        return false;
    }

    /// <inheritdoc/>
    protected override void LoadInstruction(InstructionMetadata metadata)
    {
        if (metadata.MIPSVersions.Contains(Version))
        {
            LoadInstruction(metadata.Name, metadata);
        }
        else if (!_outOfVersion.ContainsKey(metadata.Name))
        {
            _outOfVersion.Add(metadata.Name, metadata.MIPSVersions);
        }
    }
}
