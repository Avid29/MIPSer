// Avishai Dernis 2025

using Zarem.Emulator.MIPS.Components.CPU;
using Zarem.Emulator.MIPS.Components.Memory;
using Zarem.Emulator.MIPS.Config;

namespace Zarem.Emulator.MIPS.Components;

/// <summary>
/// A class representing a computer system in the MIPS interpreter.
/// </summary>
public class Computer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Computer"/> class.
    /// </summary>
    public Computer(EmulatorConfig config)
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
    public EmulatorConfig Config { get; }

    /// <summary>
    /// Advance one tick.
    /// </summary>
    /// <remarks>
    /// This is an instruction-accurate emulator, not cycle. Still, we advance in ticks.
    /// TODO: Cycle-accurate emulation.
    /// </remarks>
    public void Tick()
    {
        Processor.Step();
    }
}
