// Avishai Dernis 2026

using MIPS.Emulator.Components;
using MIPS.Emulator.Executor.Enum;
using System;

namespace MIPS.Emulator.Interpreter;

/// <summary>
/// An interpreter mimicking the MARS syscall pattern.
/// </summary>
public class MARSInterpreter : InterpreterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MARSInterpreter"/> class.
    /// </summary>
    /// <param name="computer"></param>
    public MARSInterpreter(Computer computer) : base(computer)
    {
    }

    /// <inheritdoc/>
    protected override void HandleSyscall(uint code)
    {
        switch (code)
        {
            // Print integer
            case 1:
                Console.WriteLine($"{A0}");
                break;

            // Print float
            case 2:
                // TODO:
                break;

            // Print double
            case 3:
                // TODO:
                break;

            // Print string
            case 4:
                Console.WriteLine(Computer.Memory.ReadString(A0));
                break;
        }
    }

    /// <inheritdoc/>
    protected override void HandleTrap(TrapKind trap)
    {
        throw new NotImplementedException();
    }
}
