// Avishai Dernis 2025

using System.IO;

namespace MIPS.Emulator.Models.Modules;

/// <summary>
/// An interface representing an executable module in the MIPS interpreter.
/// </summary>
public interface IExecutableModule
{
    /// <summary>
    /// Gets the entry address of the executable.
    /// </summary>
    public uint EntryAddress { get; }

    /// <summary>
    /// Loads the data module to a stream.
    /// </summary>
    /// <param name="destination">The stream to load the module to.</param>
    public void Load(Stream destination);
}
