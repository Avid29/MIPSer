// Adam Dernis 2024

using CommunityToolkit.HighPerformance;
using MIPS.Models.Instructions;
using MIPS.Models.Modules.Tables;
using MIPS.Models.Modules.Tables.Enums;

namespace MIPS.Assembler.Models.Modules;

public partial class Module
{
    /// <summary>
    /// Adds a new section to the module.
    /// </summary>
    public void AddSection(string sectionName, SectionFlags flags = SectionFlags.Default)
    {
        var section = new ModuleSection(sectionName, flags);
        _sections.Add(section.Name, section);
    }

    /// <summary>
    /// Appends an array of bytes to the end of the specified section.
    /// </summary>
    /// <remarks>
    /// Bytes must be in big endian, this is mips.
    /// </remarks>
    /// <param name="section">The section to append to.</param>
    /// <param name="bytes">The bytes to append to the end of the buffer.</param>
    /// <exception cref="ArgumentException"/>
    /// <returns>true if the data was successfully appended.</returns>
    public void Append(ModuleSection section, params byte[] bytes)
    {
        Stream buffer = section.Stream;
        buffer.Write(bytes);
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

        Append(section, stream, seekEnd);
        return true;
    }

    /// <summary>
    /// Appends the contents of a stream to the end of the specified section.
    /// </summary>
    /// <param name="section">The section to append to.</param>
    /// <param name="stream">The stream to copy.</param>
    /// <param name="seekEnd">Whether or not to seek the end of the section buffer before copying.</param>
    /// <exception cref="ArgumentException"/>
    /// <returns>true if the data was successfully appended.</returns>
    public void Append(ModuleSection section, Stream stream, bool seekEnd = true)
    {
        Stream buffer = section.Stream;
        if (seekEnd)
        {
            stream.Seek(0, SeekOrigin.Begin);
            buffer.Seek(0, SeekOrigin.End);
        }

        stream.CopyTo(buffer);
    }

    /// <summary>
    /// Aligns a section to an n-size boundary.
    /// </summary>
    /// <param name="section">The section to align.</param>
    /// <param name="boundary">The alignment boundary.</param>
    /// <returns>true if the section was successfully aligned.</returns>
    public void Align(ModuleSection section, int boundary)
    {
        // Scale boundary by power of 2.
        boundary = 1 << boundary;

        Stream stream = section.Stream;
        int offset = (int)stream.Length % boundary;

        // Already aligned
        if (offset <= 0)
            return;

        // Append offset bytes
        var append = new byte[boundary - offset];
        Append(section, append);
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
