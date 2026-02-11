// Avishai Dernis 2025

using System;
using System.IO;
using Zarem.Assembler.MIPS.Models.Modules;
using Zarem.MIPS.Models.Modules.Tables.Enums;

namespace Zarem.MIPS.Models.Modules.Tables;

/// <summary>
/// A section in the <see cref="Module"/>.
/// </summary>
public class ModuleSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleSection"/> class.
    /// </summary>
    public ModuleSection(string name, SectionFlags flags, Stream? stream = null)
    {
        Name = name;
        Flags = flags;
        Stream = stream ?? new MemoryStream();
    } 

    /// <summary>
    /// Gets the section's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the sections's flags.
    /// </summary>
    public SectionFlags Flags { get; }

    /// <summary>
    /// Gets the section stream data.
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    /// Gets or sets the address the section loads to in memory.
    /// </summary>
    public uint VirtualAddress { get; set; }

    /// <summary>
    /// Appends an array of bytes to the end of the section.
    /// </summary>
    /// <remarks>
    /// Bytes must be in big endian, this is mips.
    /// </remarks>
    /// <param name="bytes">The bytes to append to the end of the buffer.</param>
    /// <exception cref="ArgumentException"/>
    /// <returns>true if the data was successfully appended.</returns>
    public void Append(params byte[] bytes)
    {
        Stream buffer = Stream;
        buffer.Write(bytes);
    }

    /// <summary>
    /// Appends the contents of a stream to the end of the section.
    /// </summary>
    /// <param name="stream">The stream to copy.</param>
    /// <param name="seekEnd">Whether or not to seek the end of the section buffer before copying.</param>
    /// <exception cref="ArgumentException"/>
    /// <returns>true if the data was successfully appended.</returns>
    public void Append(Stream stream, bool seekEnd = true)
    {
        if (seekEnd)
        {
            stream.Seek(0, SeekOrigin.Begin);
            Stream.Seek(0, SeekOrigin.End);
        }

        stream.CopyTo(Stream);
    }

    /// <summary>
    /// Aligns a section to an n-size boundary.
    /// </summary>
    /// <param name="boundary">The alignment boundary.</param>
    /// <returns>true if the section was successfully aligned.</returns>
    public void Align(int boundary)
    {
        // Scale boundary by power of 2.
        boundary = 1 << boundary;

        Stream stream = Stream;
        int offset = (int)stream.Length % boundary;

        // Already aligned
        if (offset <= 0)
            return;

        // Append offset bytes
        var append = new byte[boundary - offset];
        Append(append);
    }
}
