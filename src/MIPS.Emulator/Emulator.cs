// Avishai Dernis 2025

using MIPS.Emulator.Components;
using MIPS.Emulator.Models.Modules;

namespace MIPS.Emulator;

/// <summary>
/// An emulator of a MIPS machine.
/// </summary>
public class Emulator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Emulator"/> class.
    /// </summary>
    public Emulator()
    {
        Computer = new Computer();
    }

    /// <summary>
    /// Gets the computer system the interpreter is emulating.
    /// </summary>
    public Computer Computer { get; }

    /// <summary>
    /// Loads an <see cref="IExecutableModule"/> to the interpreter's memory.
    /// </summary>
    /// <param name="module">The module to load.</param>
    public void Load(IExecutableModule module) => module.Load(Computer.Memory.AsStream());
}
