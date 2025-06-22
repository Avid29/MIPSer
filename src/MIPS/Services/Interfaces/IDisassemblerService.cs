﻿// Adam Dernis 2025

using MIPS.Models.Instructions;

namespace MIPS.Services.Interfaces;

#if DEBUG

/// <summary>
/// An interface for a disassembly service.
/// </summary>
public interface IDisassemblerService
{
    /// <summary>
    /// Disassembles an instruction.
    /// </summary>
    /// <param name="instruction">The instruction to disassemble.</param>
    /// <returns>The encoded instruction as assembly code.</returns>
    public string Disassemble(Instruction instruction);
}

#endif
