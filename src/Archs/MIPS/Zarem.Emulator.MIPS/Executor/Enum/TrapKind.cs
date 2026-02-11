// Avishai Dernis 2026


// Avishai Dernis 2026

using Zarem.Emulator.MIPS.Executor;

namespace Zarem.Emulator.MIPS.Executor.Enum;

/// <summary>
/// An enum describing the kind of trap that occurred during an <see cref="Execution"/>.
/// </summary>
public enum TrapKind
{

#pragma warning disable CS1591

    None,

    // Arithmetic
    ArithmeticOverflow,

    // Address / memory
    AddressErrorLoad,
    AddressErrorStore,
    AddressErrorInstruction,

    // Instruction
    ReservedInstruction,

    // Software
    Syscall,
    Breakpoint,
    Trap, // conditional trap instructions

    // Coprocessor / system
    CoprocessorUnusable,

    // MMU
    TlbMissLoad,
    TlbMissStore,
    TlbModification,

    // Interrupts
    Interrupt

#pragma warning restore CS1591
}
