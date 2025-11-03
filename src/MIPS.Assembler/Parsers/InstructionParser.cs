// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using MIPS.Assembler.Helpers.Tables;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Parsers.Enums;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Extensions;
using MIPS.Helpers;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Modules.Tables;
using MIPS.Models.Modules.Tables.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// A struct for parsing instructions.
/// </summary>
public struct InstructionParser
{
    private readonly AssemblerContext? _context;
    private readonly InstructionTable _instructionTable;
    private readonly ILogger? _logger;

    private InstructionMetadata _meta;

    private GPRegister _rs;
    private GPRegister _rt;
    private GPRegister _rd;
    private FloatFormat _format;
    private byte _shift;
    private int _immediate;
    private uint _address;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionParser"/> struct.
    /// </summary>
    public InstructionParser(InstructionTable instructions, ILogger? logger)
    {
        _instructionTable = instructions;
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionParser"/> struct.
    /// </summary>
    public InstructionParser(AssemblerContext context, ILogger? logger)
    {
        _context = context;

        _instructionTable = context.InstructionTable;
        _logger = logger;
    }

    /// <summary>
    /// Attempts to parse an instruction from a name and a list of arguments.
    /// </summary>
    /// <param name="line">The assembly line to parse.</param>
    /// <param name="parsedInstruction">The resulting <see cref="ParsedInstruction"/>.</param>
    /// <returns>Whether or not an instruction was parsed.</returns>
    public bool TryParse(AssemblyLine line, [NotNullWhen(true)] out ParsedInstruction? parsedInstruction)
    {
        ReferenceEntry? reference = null;
        parsedInstruction = null;

        // Attempt to load the instruction
        // If successful, this will set the _meta and _format
        if (!TryParseInstruction(line, out var name))
            return false;

        // Applies provided values
        _rs = (GPRegister)(_meta.RS ?? default);
        _rt = (GPRegister)(_meta.RT ?? default);
        _rd = (GPRegister)(_meta.RD ?? default);

        // Parse argument data according to pattern
        Argument[] pattern = _meta.ArgumentPattern;
        for (int i = 0; i < line.Args.Count; i++)
        {
            // Split out next arg
            var arg = line.Args[i];
            TryParseArg(arg, pattern[i], out reference);
        }

        // It's a psuedo instruction.
        // Create a pseudo-instruction and return with reference
        // as parsed instruction.
        if (_meta.IsPseudoInstruction)
        {
            Guard.IsTrue(_meta.PseudoOp.HasValue);

            var pseudo = new PseudoInstruction
            {
                PseudoOp = _meta.PseudoOp.Value,
                RS = _rs,
                RT = _rt,
                RD = _rd,
                Immediate = _immediate,
                Address = _address,
            };

            parsedInstruction = new ParsedInstruction(pseudo, reference);
            return true;
        }

        // Build an instruction using the information from
        // _meta and all the parsed arguments
        var instruction = BuildInstruction();

        // Check for write back to zero register
        // Give a warning if not an explicit nop operation
        // TODO: Check on pseudo-instructions
        if (instruction.GetWritebackRegister() is GPRegister.Zero && name != "nop")
        {
            _logger?.Log(Severity.Message, LogId.ZeroRegWriteback, "ZeroRegisterWriteback");
        }

        parsedInstruction = new ParsedInstruction(instruction, reference);
        return true;
    }

    private bool TryParseInstruction(AssemblyLine line, [NotNullWhen(true)] out string? name)
    {
        name = line.Instruction?.Source;
        Guard.IsNotNull(name);

        // Parse out format from instruction name if present
        if (FloatFormatTable.TryGetFloatFormat(name, out _format, out var formattedName))
            name = formattedName;

        if (!_instructionTable.TryGetInstruction(name, out var metas, out var version))
        {
            // Select error message
            (LogId id, string message) = version switch
            {
                // The instruction requires a higher MIPS version
                not null when _context is null || version > _context?.Config.MipsVersion =>
                    (LogId.NotInVersion, $"The instruction '{name}' requires mips version {version:d}."),

                // The instruction is deprecated
                not null => (LogId.NotInVersion, $"The instruction '{name}' is deprecated. Last supported in mips version {version:d}."),

                // The instruction does not exist.
                null => (LogId.InvalidInstructionName, $"No instruction named '{name}'.")
            };

            // Log the error
            // TODO: Improve version formatting
            _logger?.Log(Severity.Error, id, message, name, $"{version:d}");
            return false;
        }

        // Assert instruction metadata with proper argument count exists
        if (!metas.Any(x => x.ArgumentPattern.Length == line.Args.Count))
        {
            // TODO: Improve messaging
            //var message = line.Args.Count < pattern.Length
            //    ? $"Instruction '{name}' doesn't have enough arguments. Found {line.Args.Count} arguments when expecting {_meta.ArgumentPattern.Length}."
            //    : $"Instruction '{name}' has too many arguments! Found {line.Args.Count} arguments when expecting {_meta.ArgumentPattern.Length}.";

            _logger?.Log(Severity.Error, LogId.InvalidInstructionArgCount, "WrongArgumentCount", name, line.Args.Count);
            return false;
        }

        // Find instruction pattern with matching argument count
        _meta = metas.FirstOrDefault(x => x.ArgumentPattern.Length == line.Args.Count);

        // Check that the float format is supported valid with the instruction, if applicable
        if (_meta.FloatFormats is not null && !_meta.FloatFormats.Contains(_format))
        {
            _logger?.Log(Severity.Error, LogId.InvalidFloatFormat, $"DoesNotSupportFormat{_format}.", name);
            return false;
        }

        return true;
    }

    private bool TryParseArg(ReadOnlySpan<Token> arg, Argument type, out ReferenceEntry? reference)
    {
        reference = null;

        return type switch
        {
            // Register arguments
            (>= Argument.RS and <= Argument.RD) or
            (>= Argument.FS and <= Argument.FD) or
            Argument.RT_Numbered => TryParseRegisterArg(arg, type),

            // Expression arguments
            Argument.Shift or Argument.Immediate or
            Argument.FullImmediate or Argument.Offset
            or Argument.Address => TryParseExpressionArg(arg, type, out reference),

            // Address offset arguments
            Argument.AddressBase => TryParseAddressOffsetArg(arg, out reference),

            _ => ThrowHelper.ThrowArgumentOutOfRangeException<bool>($"Argument of type '{type}' is not within parsable type range."),
        };
    }

    /// <summary>
    /// Parses an argument as a register and assigns it to the target component.
    /// </summary>
    private unsafe bool TryParseRegisterArg(ReadOnlySpan<Token> arg, Argument target)
    {
        if (arg.Length is not 1)
        {
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, "ArgumentNotARegister", arg.Print());
            return false;
        }

        // Get reference to selected register argument
        RefTuple<Ref<GPRegister>, RegisterSet> pair = target switch
        {
            // General Purpose Registers
            Argument.RS => new(new(ref _rs), RegisterSet.GeneralPurpose),
            Argument.RT => new(new(ref _rt), RegisterSet.GeneralPurpose),
            Argument.RD => new(new(ref _rd), RegisterSet.GeneralPurpose),
            // Float Registers
            Argument.FS => new(new(ref _rs), RegisterSet.FloatingPoints),
            Argument.FT => new(new(ref _rt), RegisterSet.FloatingPoints),
            Argument.FD => new(new(ref _rd), RegisterSet.FloatingPoints),
            // RT Register for coprocessors
            Argument.RT_Numbered => new(new(ref _rt), RegisterSet.Numbered),
            // Invalid target type
            _ => throw new ArgumentOutOfRangeException($"Argument of type '{target}' attempted to parse as a register.")
        };

        (Ref<GPRegister> regRef, RegisterSet set) = pair;
        ref GPRegister reg = ref regRef.Value;

        if (!TryParseRegister(arg[0], out var register, set))
        {
            // Register could not be parsed.
            // Error already logged.

            return false;
        }

        // Cache register as appropriate argument type
        reg = register;

        return true;
    }

    /// <summary>
    /// Parses an argument as an expression and assigns it to the target component
    /// </summary>
    private bool TryParseExpressionArg(ReadOnlySpan<Token> arg, Argument target, out ReferenceEntry? relocation)
    {
        relocation = null;
        var parser = new ExpressionParser(_context, _logger);

        // Attempt to parse expression
        if (!parser.TryParse(arg, out var address, out SymbolEntry? refSymbol))
            return false;

        if (!address.IsFixed && target is Argument.Shift)
        {
            _logger?.Log(Severity.Error, LogId.RelocatableReferenceInShift, "Shift amount argument cannot reference relocatable symbols.");
            return false;
        }

        // TODO: Can branches make external references?

        if (!address.IsFixed && target is not Argument.Offset && _context is not null)
        {
            Guard.IsNotNull(refSymbol);

            var type = target switch
            {
                Argument.Address => ReferenceType.Address,
                Argument.Immediate => ReferenceType.Lower,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<ReferenceType>($"Argument of type '{target}' cannot reference relocatable symbols."),
            };

            var method = ReferenceMethod.Relocate;
            if (address.IsExternal)
            {
                // TODO: When is it replace or subtract?
                method = ReferenceMethod.Add;
            }

            relocation = new ReferenceEntry(refSymbol.Value.Name, _context.CurrentAddress, type, method);
        }

        // NOTE: Casting might truncate the value to fit the bit size.
        // This is the desired behavior, but when logging errors this
        // should be handled explicitly and drop an assembler warning.

        long value = address.Value;

        // Truncates the value to fit the target argument
        CleanInteger(ref value, arg, target);

        // Assign to appropriate instruction argument
        switch (target)
        {
            case Argument.Shift:
                _shift = (byte)value;
                return true;
            case Argument.Immediate:
                _immediate = (short)value;
                return true;
            case Argument.FullImmediate:
                _immediate = (int)value;
                return true;
            case Argument.Address:
                _address = (uint)value;
                return true;
            case Argument.Offset:
                if (address.IsRelocatable)
                {
                    Guard.IsNotNull(_context);

                    var @base = _context.CurrentAddress + 4;
                    if (@base.Section != address.Section)
                    {
                        _logger?.Log(Severity.Error, LogId.BranchBetweenSections, "CantBranchBetweenSections");
                        return false;
                    }

                    // Adjust realtive to current position
                    value -= @base.Value;
                }

                _immediate = (int)value;
                return true;

            // Invalid target type
            default:
                return ThrowHelper.ThrowArgumentOutOfRangeException<bool>($"Argument '{arg.Print()}' of type '{target}' attempted to parse as an expression.");
        }
    }

    /// <summary>
    /// Parses an argument as an address offset, assigning its components to immediate and $rs.
    /// </summary>
    private bool TryParseAddressOffsetArg(ReadOnlySpan<Token> arg, out ReferenceEntry? relSymbol)
    {
        relSymbol = null;

        // NOTE: Be careful about forwards to other parse functions with regards to 
        // error logging. Address offset argument errors might be inappropriately logged.

        // Split the string into an offset and a register, return false if failed
        if (!SplitAddressOffset(arg, out var offsetStr, out var regStr))
            return false;

        // Try parse offset component into immediate, return false if failed
        if (!TryParseExpressionArg(offsetStr, Argument.Immediate, out relSymbol))
            return false;

        // Parse register component into $rs, return false if failed
        if (!TryParseRegisterArg(regStr, Argument.RS))
            return false;

        return true;
    }

    private readonly bool TryParseRegister(Token arg, out GPRegister register, RegisterSet set = RegisterSet.GeneralPurpose)
    {
        register = GPRegister.Zero;

        // Check that argument is register argument
        var regStr = arg.Source;
        if (regStr[0] != '$')
        {
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, "ArgumentNotARegister", arg);
            return false;
        }

        // Get named register from table
        if (!RegistersTable.TryGetRegister(regStr, out register, out RegisterSet parsedSet))
        {
            // Register does not exist in table
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, "RegisterNotFound", arg);
            return false;
        }

        // Match register set
        if (parsedSet != RegisterSet.Numbered && parsedSet != set)
        {
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"RegisterMustBeIn{set}Set", arg);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Splits an address offset argument into a token span for the offset and the address register token.
    /// </summary>
    /// <remarks>
    /// Upon return offset and register do not need to be valid offset and register strings.
    /// The register is just the component in parenthesis. The offset is just the component before the parenthesis.
    /// Nothing may follow the parenthesis.
    /// </remarks>
    private readonly bool SplitAddressOffset(ReadOnlySpan<Token> arg, out ReadOnlySpan<Token> offset, out ReadOnlySpan<Token> register)
    {
        offset = arg;
        register = [];

        // Find matched parenthesis start and end
        var parIndex = arg.FindNext(TokenType.OpenParenthesis);
        var closeIndex = arg.FindNext(TokenType.CloseParenthesis);
        if (parIndex is -1 || closeIndex is -1)
        {
            // TODO: Improve messaging
            _logger?.Log(Severity.Error, LogId.InvalidAddressOffsetArgument, "InvalidAddressOffsetArgument", arg.Print());
            return false;
        }

        // Offset is everything before the parenthesis
        offset = arg[..parIndex];

        // Register is everything between the parenthesis
        register = arg[(parIndex+1)..closeIndex];

        // Ensure there's no content following the parenthesis.
        if (!arg[(closeIndex+1)..].IsEmpty)
        {
            // TODO: Improve messaging
            _logger?.Log(Severity.Error, LogId.InvalidAddressOffsetArgument, "InvalidAddressOffsetArgument", arg.Print());
            return false;
        }

        return true;
    }

    private void CleanInteger(ref long value, ReadOnlySpan<Token> arg, Argument target)
    {
        // Determine casting details for the argument
        (int bitCount, int shiftAmount, bool signed) = target switch
        {
            Argument.Shift => (5, 0, false),
            Argument.Offset => (16, 2, false),
            Argument.Immediate => (16, 0, true),
            Argument.Address => (26, 2, false),
            Argument.FullImmediate => (32, 0, true),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<(byte, byte, bool)>($"Argument of type '{target}' attempted to parse as an expression."),
        };

        // Clean integer to fit within argument bit size and match signs
        long original = value;
        var cleanStatus = CastInteger(ref value, bitCount, shiftAmount, signed);

        // Log a message if the value was truncated and/or had its sign changed
        if (cleanStatus is not CastingChanges.None)
        {
            _logger?.Log(Severity.Warning, LogId.IntegerTruncated, $"CastWarning{cleanStatus}", arg.Print(), original, value, bitCount, shiftAmount);
        }
    }

    /// <remarks>
    /// This does not apply the <paramref name="shiftAmount"/>! It only masks the lower bits.
    /// </remarks>
    /// <param name="integer">A reference to the integer to modify.</param>
    /// <param name="bitCount">The number of bits after casting.</param>
    /// <param name="shiftAmount">The number of bits that will drop from the bottom.</param>
    /// <param name="signed">Whether or not the new value should be signed.</param>
    /// <returns>The changes made to the integer.</returns>
    private static CastingChanges CastInteger(ref long integer, int bitCount, int shiftAmount, bool signed = false)
    {
        var original = integer;

        Guard.IsGreaterThan(bitCount, 1);
        Guard.IsLessThanOrEqualTo(bitCount + shiftAmount, 64);

        // Create a masks for the high and low truncating bits,
        // as well as an overall remaining bits map
        var upperMask = bitCount == 64 ? -1L : (1L << (bitCount + shiftAmount)) - 1;
        var lowerMask = ~((1L << shiftAmount) - 1);
        var mask = (upperMask & lowerMask);

        // Truncate mask upper and lower bits
        long truncated = integer & mask;

        // Sign extend if signed and not full width
        if (signed && bitCount < 64)
        {
            long signBit = 1L << (bitCount - 1);
            if ((truncated & signBit) != 0)
                truncated |= ~upperMask; // Sign extend
        }

        integer = truncated;

        // Compute changes
        var changes = CastingChanges.None;

        // Check if the sign was dropped
        if (!signed && original < 0)
            changes |= CastingChanges.SignChanged;

        // Check for upper truncation
        long upperBits = original & ~upperMask;
        if (upperBits != 0 && upperBits != ~upperMask)
            changes |= CastingChanges.TruncatedHigh;

        // Check for lower truncation
        if ((original & ~lowerMask) != 0)
        {
            changes |= CastingChanges.TruncatedLow;
        }

        // Return combined code
        return changes;
    }

    private readonly Instruction BuildInstruction()
    {
        // If it's not a pseudo instruction, there should be an OpCode
        Guard.IsNotNull(_meta.OpCode);

        // Create the instruction from its components based on the instruction type
        return _meta.OpCode switch
        {
            // R Type
            OperationCode.Special => _meta.FuncCode.HasValue ?                              // Special
                Instruction.Create(_meta.FuncCode.Value, _rs, _rt, _rd, _shift) :
                _ = ThrowHelper.ThrowArgumentException<Instruction>($"Instructions with OpCode:{_meta.OpCode} must have a {nameof(_meta.FuncCode)} value."),
            OperationCode.Special2 => _meta.Function2Code.HasValue ?                        // Special 2
                Instruction.Create(_meta.Function2Code.Value, _rs, _rt, _rd, _shift) :
                _ = ThrowHelper.ThrowArgumentException<Instruction>($"Instructions with OpCode:{_meta.OpCode} must have a {nameof(_meta.Function2Code)} value."),
            OperationCode.Special3 => _meta.Function3Code.HasValue ?                        // Special 3
                Instruction.Create(_meta.Function3Code.Value, _rs, _rt, _rd, _shift) :
                _ = ThrowHelper.ThrowArgumentException<Instruction>($"Instructions with OpCode:{_meta.OpCode} must have a {nameof(_meta.Function3Code)} value."),

            // J Type
            OperationCode.Jump or OperationCode.JumpAndLink
            or OperationCode.JumpAndLinkX => Instruction.Create(_meta.OpCode.Value, _address),

            // Coprocessor0 instructions
            OperationCode.Coprocessor0 when _meta.Co0FuncCode.HasValue                      // C0
                => CoProc0Instruction.Create(_meta.Co0FuncCode.Value),
            OperationCode.Coprocessor0 when _meta.Mfmc0FuncCode.HasValue                    // MFMC0
                => CoProc0Instruction.Create(_meta.Mfmc0FuncCode.Value, _rt, _meta.RD),
            OperationCode.Coprocessor0 => _meta.CoProc0RS.HasValue ?                        // Co0 RS
                CoProc0Instruction.Create(_meta.CoProc0RS.Value, _rt, _rd) :
                _ = ThrowHelper.ThrowArgumentException<Instruction>($"Instructions with OpCode:{_meta.OpCode} must have a {nameof(_meta.CoProc0RS)}, {nameof(_meta.Co0FuncCode)}, or {nameof(_meta.Mfmc0FuncCode)} value."),

            // FloatingPoint instructions
            OperationCode.Coprocessor1 when _meta.FloatFuncCode.HasValue && _meta.FloatFormats is not null  // Floating-Point
                => FloatInstruction.Create(_meta.FloatFuncCode.Value, _format, (FloatRegister)_rs, (FloatRegister)_rd, (FloatRegister)_rt),
            OperationCode.Coprocessor1 => _meta.CoProc1RS.HasValue ?                                    // CoProc1
                FloatInstruction.Create(_meta.CoProc1RS.Value, _rt, (FloatRegister)_rs) :
                _ = ThrowHelper.ThrowArgumentException<Instruction>($"Instruction with OpCode:{_meta.OpCode} must have a {nameof(_meta.CoProc1RS)} or {nameof(_meta.FloatFuncCode)} value."),

            // Register Immediate
            OperationCode.RegisterImmediate => _meta.RegisterImmediateFuncCode switch
            {
                // Register Immediate Branching
                (>= RegImmFuncCode.BranchOnLessThanZero and <= RegImmFuncCode.BranchOnGreaterThanZeroLikely) or
                (>= RegImmFuncCode.BranchOnLessThanZeroAndLink and <= RegImmFuncCode.BranchOnGreaterThanOrEqualToZeroLikelyAndLink)
                    => Instruction.Create(_meta.RegisterImmediateFuncCode.Value, _rs, _immediate),

                // Throw exception if null
                null => ThrowHelper.ThrowArgumentException<Instruction>($"Instruction with OpCode:{_meta.OpCode} must have a {nameof(_meta.RegisterImmediateFuncCode)} value."),

                // Register Immediate
                _ => Instruction.Create(_meta.RegisterImmediateFuncCode.Value, _rs, (short)_immediate)
            },

            // I-Type Branch
            (>= OperationCode.BranchOnEquals and <= OperationCode.BranchOnGreaterThanZero) or
            (>= OperationCode.BranchOnEqualLikely and <= OperationCode.BranchOnGreaterThanZeroLikely)
                    => Instruction.Create(_meta.OpCode.Value, _rs, _rt, _immediate),

            // Remaining I Type instructions
            _ => Instruction.Create(_meta.OpCode.Value, _rs, _rt, (short)_immediate),
        };
    }
}
