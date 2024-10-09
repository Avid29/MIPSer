// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing.Enums;
using System;
using System.IO;

namespace MIPS.Assembler.Models.Modules;

public partial class ModuleConstruction
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

    private Stream GetSectionStream(Section section)
    {
        if (section is < Section.Text or > Section.UninitializedData)
            ThrowHelper.ThrowArgumentOutOfRangeException($"Section must be between {Section.Text} and {Section.UninitializedData}.");

        return _sections[(int)section];
    }
}
