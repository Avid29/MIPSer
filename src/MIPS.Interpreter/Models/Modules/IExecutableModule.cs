// Avishai Dernis 2025

using System.IO;

namespace MIPS.Interpreter.Models.Modules;

/// <summary>
/// An interface representing an executable module in the MIPS interpreter.
/// </summary>
public interface IExecutableModule
{
    /// <summary>
    /// Gets the contents of the executable.
    /// </summary>
    public Stream Contents { get; }

    /// <summary>
    /// Gets the entry address of the executable.
    /// </summary>
    public uint EntryAddress { get; }
}
