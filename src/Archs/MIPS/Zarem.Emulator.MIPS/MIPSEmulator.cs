// Avishai Dernis 2025

using Zarem.Emulator.Config;
using Zarem.Emulator.Machine;
using Zarem.Emulator.Models.Enums;
using Zarem.Emulator.Models.Modules;

namespace Zarem.Emulator;

/// <summary>
/// An emulator of a MIPS machine.
/// </summary>
public class MIPSEmulator : Emulator<MIPSEmulatorConfig>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MIPSEmulator"/> class.
    /// </summary>
    public MIPSEmulator(MIPSEmulatorConfig config)
    {
        Computer = new Machine.MIPSComputer(config);
    }

    /// <summary>
    /// Gets the computer system the interpreter is emulating.
    /// </summary>
    public MIPSComputer Computer { get; }

    /// <summary>
    /// Loads an <see cref="IExecutableModule"/> to the interpreter's memory.
    /// </summary>
    /// <param name="module">The module to load.</param>
    public override void Load(IExecutableModule module)
    {
        module.Load(Computer.Memory.AsStream());
        State = EmulatorState.Ready;
    }

    /// <inheritdoc/>
    protected override void Tick() => Computer.Tick();
}
