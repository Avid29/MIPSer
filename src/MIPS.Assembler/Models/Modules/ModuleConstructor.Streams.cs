// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using MIPS.Models.Addressing.Enums;
using MIPS.Models.Instructions;
using MIPS.Models.Modules.Tables;
using MIPS.Models.Modules.Tables.Enums;
using System;
using System.IO;

namespace MIPS.Assembler.Models.Modules;

public partial class ModuleConstructor
{
    /// <summary>
    /// Gets the current position in the text stream.
    /// </summary>
    public long TextPosition => Text.Position;

    /// <summary>
    /// Gets the current position in the data stream.
    /// </summary>
    public long DataPosition => Data.Position;

    /// <summary>
    /// Appends an array of bytes to the end of the specified section.
    /// </summary>
    /// <remarks>
    /// Bytes must be in big endian, this is mips.
    /// </remarks>
    /// <param name="section">The segment to append to</param>
    /// <param name="bytes">The bytes to append to the end of the buffer.</param>
    /// <exception cref="ArgumentException"/>
    public void Append(Section section, params byte[] bytes)
    {
        // Select buffer and write bytes
        Stream buffer = GetSectionStream(section);
        buffer.Write(bytes);
    }
    
    /// <summary>
    /// Appends the contents of a stream to the end of the specified section.
    /// </summary>
    /// <param name="section">The segment to append to</param>
    /// <param name="stream">The stream to copy.</param>
    /// <param name="seekEnd">Whether or not to seek the end of the section buffer before copying.</param>
    /// <exception cref="ArgumentException"/>
    public void Append(Section section, Stream stream, bool seekEnd = true)
    {
        // Select buffer and copy
        Stream buffer = GetSectionStream(section);
        if (seekEnd)
        {
            buffer.Seek(0, SeekOrigin.End);
        }

        stream.CopyTo(buffer);
    }

    /// <summary>
    /// Aligns a section to an n-size boundary.
    /// </summary>
    /// <param name="section">The section to align.</param>
    /// <param name="boundary">The alignment boundary.</param>
    public void Align(Section section, int boundary)
    {
        // Scale boundary by power of 2.
        boundary = 1 << boundary;

        // Select buffer and get alignment offset
        Stream stream = GetSectionStream(section);
        int offset = (int)stream.Length % boundary;

        // Already aligned
        if (offset <= 0)
            return;

        // Append offset bytes
        var append = new byte[offset];
        Append(section, append);
    }

    /// <summary>
    /// Gets the position of a section stream.
    /// </summary>
    /// <param name="section">The section position to retrieve .</param>
    /// <returns>The position of the given section's stream position.</returns>
    public long GetStreamPosition(Section section)
    {
        if (section is < Section.Text or > Section.UninitializedData)
            ThrowHelper.ThrowArgumentOutOfRangeException($"Section must be between {Section.Text} and {Section.UninitializedData}.");

        return _sections[(int)section].Position;
    }

    /// <summary>
    /// Seeks to the start of all sections.
    /// </summary>
    public void ResetStreamPositions()
    {
        foreach (var section in _sections)
            section.Position = 0;
    }

    /// <summary>
    /// Applies a relocation.
    /// </summary>
    /// <param name="entry">The reference entry detailing the relocation.</param>
    /// <param name="offset">The offset amount of the relocation.</param>
    public void Relocate(ReferenceEntry entry, long offset)
    {
        // Read initial value
        Stream stream = GetSectionStream(entry.Address.Section);
        stream.Seek(entry.Address.Value, SeekOrigin.Begin);
        var word = stream.Read<uint>();

        // Apply relocation change
        switch (entry.Type)
        {
            case ReferenceType.FullWord:
                word += (uint)offset;
                break;
            case ReferenceType.Address:
                // Kinda round about way to handle it, but this should work quite well
                var instr = (Instruction)word;
                word = (uint)Instruction.Create(instr.OpCode, instr.Address + (uint)offset);
                break;
            case ReferenceType.Lower:
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

    private Stream GetSectionStream(Section section)
    {
        if (section is < Section.Text or > Section.UninitializedData)
            ThrowHelper.ThrowArgumentOutOfRangeException($"Section must be between {Section.Text} and {Section.UninitializedData}.");

        return _sections[(int)section];
    }
}
