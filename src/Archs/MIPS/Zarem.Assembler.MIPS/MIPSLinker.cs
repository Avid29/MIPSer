// Adam Dernis 2024

using CommunityToolkit.HighPerformance;
using System.IO;
using Zarem.Models.Instructions;
using Zarem.Models.Modules;
using Zarem.Models.Modules.Tables;
using Zarem.Models.Modules.Tables.Enums;

namespace Zarem.Assembler;

/// <summary>
/// A MIPS linker.
/// </summary>
public class MIPSLinker : Linker<MIPSLinker>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSLinker"/> class.
    /// </summary>
    public MIPSLinker()
    {
    }

    /// <inheritdoc/>
    protected override void Relocate(ReferenceEntry entry, long offset)
    {
        // Get section
        var sectionName = entry.Location.Section;
        if (sectionName is null || !Module.TryGetSection(sectionName, out var section))
            return;

        // Read initial value
        Stream stream = section.Stream;
        stream.Seek(entry.Location.Value, SeekOrigin.Begin);
        var word = stream.Read<uint>();

        // Apply relocation change
        switch (entry.Type)
        {
            case MipsReferenceType.Absolute32:
                word += (uint)offset;
                break;
            case MipsReferenceType.JumpTarget26:
                // Kinda round about way to handle it, but this should work quite well
                var instr = (MIPSInstruction)word;
                word = (uint)MIPSInstruction.Create(instr.OpCode, instr.Address + (uint)offset);
                break;
            case MipsReferenceType.Low16:
                uint mask = (1 << 16) - 1;
                var lower = (word & mask) + (uint)offset;
                word = (lower & mask) + (word & ~mask);
                break;
        }

        // Overwrite the value
        stream.Seek(-sizeof(uint), SeekOrigin.Current);
        stream.Write(word);

        // Return to end of stream
        stream.Seek(0, SeekOrigin.End);
    }
}
