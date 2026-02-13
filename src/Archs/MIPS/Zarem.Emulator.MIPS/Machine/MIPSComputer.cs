// Avishai Dernis 2025

using Zarem.Emulator.Machine.CPU;
using Zarem.Emulator.Machine.Memory;
using Zarem.Emulator.Config;

namespace Zarem.Emulator.Machine;

/// <summary>
/// A class representing a computer system in the MIPS interpreter.
/// </summary>
public class MIPSComputer : IComputer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSComputer"/> class.
    /// </summary>
    public MIPSComputer(MIPSEmulatorConfig config)
    {
        Config = config;

        Processor = new Processor(this);
        Memory = new SystemMemory();
    }

    /// <summary>
    /// Gets the processor of the computer system.
    /// </summary>
    public Processor Processor { get; }

    /// <summary>
    /// Gets the memory of the computer system.
    /// </summary>
    public SystemMemory Memory { get; }

    /// <summary>
    /// Gets the emulation configuration to follow for computing.
    /// </summary>
    public MIPSEmulatorConfig Config { get; }

    /// <inheritdoc/>
    public void Tick()
    {
        Processor.Step();
    }
}
