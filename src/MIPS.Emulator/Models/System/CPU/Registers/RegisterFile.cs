// Avishai Dernis 2025

using MIPS.Models.Instructions.Enums.Registers;

namespace MIPS.Emulator.Models.System.CPU.Registers;

/// <summary>
/// A class representing a register file.
/// </summary>
public class RegisterFile
{
    private readonly uint[] _registers;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterFile"/> class.
    /// </summary>
    public RegisterFile(int count = 32)
    {
        _registers = new uint[count];
    }
    
    /// <summary>
    /// Gets or sets the value in a register.
    /// </summary>
    public uint this[int register]
    {
        get => _registers[register];
        set
        {
            // Register is out of the indexable bounds. Do nothing.
            if (register < 0 || register >= _registers.Length)
                return;

            _registers[register] = value;
        }
    }

    /// <summary>
    /// Gets or sets the value in a register.
    /// </summary>
    public uint this[GPRegister register]
    {
        get => this[(int)register];
        set
        {
            // Cannot set zero register. Do nothing.
            if (register is GPRegister.Zero)
                return;

            this[(int)register] = value;
        }
    }

    /// <inheritdoc cref="this[GPRegister]"/>
    public uint this[CP0Registers register]
    {
        get => this[(int)register];
        set => this[(int)register] = value;
    }
}
