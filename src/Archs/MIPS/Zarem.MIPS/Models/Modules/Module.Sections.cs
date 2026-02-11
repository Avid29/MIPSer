// Adam Dernis 2024

using CommunityToolkit.HighPerformance;
using System;
using System.IO;
using Zarem.MIPS.Models.Instructions;
using Zarem.MIPS.Models.Modules.Tables;
using Zarem.MIPS.Models.Modules.Tables.Enums;

namespace Zarem.Assembler.MIPS.Models.Modules;

public partial class Module
{
    /// <summary>
    /// Adds a new section to the module.
    /// </summary>
    public ModuleSection AddSection(string sectionName, SectionFlags flags = SectionFlags.Default)
    {
        var section = new ModuleSection(sectionName, flags);
        _sections.Add(section.Name, section);
        return section;
    }
    
    /// <summary>
    /// Gets a section, or creates it if it does not exist.
    /// </summary>
    /// <param name="sectionName">The name of the section to get.</param>
    /// <returns>A section with the provided name</returns>
    public ModuleSection GetOrAddSection(string sectionName)
    {
        if (!_sections.TryGetValue(sectionName, out var section))
            section = AddSection(sectionName);

        return section;
    }

    /// <summary>
    /// Appends the contents of a stream to the end of the specified section.
    /// </summary>
    /// <param name="sectionName">The name of the section to append to.</param>
    /// <param name="stream">The stream to copy.</param>
    /// <param name="seekEnd">Whether or not to seek the end of the section buffer before copying.</param>
    /// <exception cref="ArgumentException"/>
    /// <returns>true if the data was successfully appended.</returns>
    public bool Append(string sectionName, Stream stream, bool seekEnd = true)
    {
        if (!_sections.TryGetValue(sectionName, out var section))
            return false;

        section.Append(stream, seekEnd);
        return true;
    }

    /// <summary>
    /// Seeks to the start of all sections.
    /// </summary>
    public void ResetStreamPositions()
    {
        foreach (var section in _sections.Values)
            section.Stream.Position = 0;
    }

    /// <summary>
    /// Applies a relocation.
    /// </summary>
    /// <param name="entry">The reference entry detailing the relocation.</param>
    /// <param name="offset">The offset amount of the relocation.</param>
    public void Relocate(ReferenceEntry entry, long offset)
    {
        // Get section
        var sectionName = entry.Location.Section;
        if (sectionName is null ||!_sections.TryGetValue(sectionName, out var section))
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
                var instr = (Instruction)word;
                word = (uint)Instruction.Create(instr.OpCode, instr.Address + (uint)offset);
                break;
            case MipsReferenceType.Low16:
                uint mask = (1 << 16)-1;
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
