// Adam Dernis 2024

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Zarem.Models.Modules.Tables;
using Zarem.Models.Modules.Tables.Enums;

namespace Zarem.Models.Modules;

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
    /// Attempts to retrieve a <see cref="ModuleSection"/> by name.
    /// </summary>
    /// <param name="sectionName">The name of the section to the retrieve.</param>
    /// <param name="section">The section, if it exists.</param>
    /// <returns>True if the section was found, false otherwise.</returns>
    public bool TryGetSection(string sectionName, [NotNullWhen(true)] out ModuleSection? section)
        => _sections.TryGetValue(sectionName, out section);

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
}
