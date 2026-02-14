// Avishai Dernis 2026

namespace Zarem.Disassembler.Models;

/// <summary>
/// A type describing the info to lookup an instruction in a disassembler instruction table.
/// </summary>
public record struct DisassemblerLookup(byte OpCode, byte FuncCode, byte FuncCode2, bool IsFloat);
