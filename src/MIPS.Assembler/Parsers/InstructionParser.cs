// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Helpers.Tables;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Extensions;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Modules.Tables;
using MIPS.Models.Modules.Tables.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

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

    private Register _rs;
    private Register _rt;
    private Register _rd;
    private FloatFormat _format;
    private byte _shift;
    private int _immediate;
    private uint _address;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionParser"/> struct.
    /// </summary>
    public InstructionParser(InstructionTable instructions, ILogger? logger) : this(context:null, logger)
    {
        _instructionTable = instructions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionParser"/> struct.
    /// </summary>
    public InstructionParser(AssemblerContext? context, ILogger? logger)
    {
        _context = context;

        // This contains a null suppresion so the instructionTable can
        // be set by the calling constructor. It's not great design.
        _instructionTable = context?.InstructionTable ?? null!; 
        _logger = logger;
        _meta = default;
        _rs = default;
        _rt = default;
        _rd = default;
        _shift = default;
        _immediate = default;
        _address = default;
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

        var name = line.Instruction?.Source;
        Guard.IsNotNull(name);

        // Parse out format from instruction name if present
        if (FloatFormatTable.TryGetFloatFormat(name, out _format, out var lookupName))
            name = lookupName;

        // Get instruction metadata from name
        if (!_instructionTable.TryGetInstruction(name, out _meta, out var version))
        {
            if (version is not null)
            {
                // The instruction exists, but is not supported with the active MIPS version.
                if (_context is null || version > _context?.Config.MipsVersion)
                {
                    _logger?.Log(Severity.Error, LogId.NotInVersion, $"The instruction '{name}' requires mips version {version:d}.");
                } else
                {
                    _logger?.Log(Severity.Error, LogId.NotInVersion, $"The instruction '{name}' is deprecated. Last supported in mips version {version:d}.");
                }
            }
            else
            {
                // The instruction does not exist in the table.
                _logger?.Log(Severity.Error, LogId.InvalidInstructionName, $"No instruction named '{name}'.");
            }
            return false;
        }

        // Check that the float format is supported valid with the instruction, if applicable
        if (_meta.FloatFormats is not null && !_meta.FloatFormats.Contains(_format))
        {
            _logger?.Log(Severity.Error, LogId.InvalidFloatFormat, $"Instruction '{name}' does not support float format '{_format}'.");
            return false;
        }

        // Parse argument data according to pattern
        Argument[] pattern = _meta.ArgumentPattern;
        
        // Assert proper argument count for instruction
        if (line.Args.Count != pattern.Length)
        {
            var message = line.Args.Count < pattern.Length
                ? $"Instruction '{name}' doesn't have enough arguments. Found {line.Args.Count} arguments when expecting {_meta.ArgumentPattern.Length}."
                : $"Instruction '{name}' has too many arguments! Found {line.Args.Count} arguments when expecting {_meta.ArgumentPattern.Length}.";

            _logger?.Log(Severity.Error, LogId.InvalidInstructionArgCount, message);
            return false;
        }

        for (int i = 0; i < line.Args.Count; i++)
        {
            // Split out next arg
            var arg = line.Args[i];
            TryParseArg(arg, pattern[i], out reference);
        }

        // Handle the pseudo-instruction condition
        if (_meta.IsPseudoInstruction)
        {
            Guard.IsTrue(_meta.PseudoOp.HasValue);

            var pseudo = new PseudoInstruction()
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

        // If it's not a pseudo instruction, there should be an OpCode
        Guard.IsNotNull(_meta.OpCode);

        // Create the instruction from its components based on the instruction type
        Instruction instruction = _meta.OpCode switch
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
            (>= OperationCode.BranchOnEquals and <= OperationCode.BranchGreaterThanZero) or
            (>= OperationCode.BranchOnEqualLikely and <= OperationCode.BranchOnGreaterThanZeroLikely)
                    => Instruction.Create(_meta.OpCode.Value, _rs, _rt, _immediate),

            // Remaining I Type instructions
            _ => Instruction.Create(_meta.OpCode.Value, _rs, _rt, (short)_immediate),
        };

        // Check for write back to zero register
        // Give a warning if not an explicit nop operation
        // TODO: Check on pseudo-instructions
        if (instruction.GetWritebackRegister() is Register.Zero && name != "nop")
        {
            _logger?.Log(Severity.Message, LogId.ZeroRegWriteBack, "This instruction writes to $zero.");
        }

        parsedInstruction = new ParsedInstruction(instruction, reference);
        return true;
    }

    private bool TryParseArg(ReadOnlySpan<Token> arg, Argument type, out ReferenceEntry? reference)
    {
        reference = null;

        return type switch
        {
            // Register arguments
            (>= Argument.RS and <= Argument.RD) or (>= Argument.FS and <= Argument.FD) or Argument.RT_Numbered => TryParseRegisterArg(arg[0], type),
            // Expression arguments
            Argument.Shift or Argument.Immediate or Argument.FullImmediate or Argument.Offset or Argument.Address => TryParseExpressionArg(arg, type, out reference),
            // Address offset arguments
            Argument.AddressBase => TryParseAddressOffsetArg(arg, out reference),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<bool>($"Argument of type '{type}' is not within parsable type range."),
        };
    }

    /// <summary>
    /// Parses an argument as a register and assigns it to the target component.
    /// </summary>
    private unsafe bool TryParseRegisterArg(Token arg, Argument target)
    {
        // Get reference to selected register argument
        ref Register reg = ref _rs;
        RegisterSet set = RegisterSet.GeneralPurpose;
        switch (target)
        {
            // GPU Registers
            case Argument.RS:
                reg = ref _rs;
                break;
            case Argument.RT:
                reg = ref _rt;
                break;
            case Argument.RD:
                reg = ref _rd;
                break;

            // Float Registers
            case Argument.FS:
                reg = ref _rs;
                set = RegisterSet.FloatingPoints;
                break;
            case Argument.FT:
                reg = ref _rt;
                set = RegisterSet.FloatingPoints;
                break;
            case Argument.FD:
                reg = ref _rd;
                set = RegisterSet.FloatingPoints;
                break;

            // RT Register for coprocessors
            case Argument.RT_Numbered:
                reg = ref _rt;
                set = RegisterSet.Numbered;
                break;

            // Invalid target type
            default:
                // TODO: improve message
                return ThrowHelper.ThrowArgumentOutOfRangeException<bool>($"Argument of type '{target}' attempted to parse as a register.");
        }

        if (!TryParseRegister(arg, out var register, set))
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

        // NOTE: Casting might truncate the value to fit the bit size.
        // This is the desired behavior, but when logging errors this
        // should be handled explicitly and drop an assembler warning.

        // Determine the bits allowed by the 
        int bitCount = target switch
        {
            Argument.Shift => 5,
            Argument.Immediate => 16,
            Argument.Offset => 18,  // Offset and address are only 16/26 bits in storage. However, the last
            Argument.Address => 28, // two bits are dropped so the 18th and 28th bits must be cleaned too
            Argument.FullImmediate => 32,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<int>($"Argument of type '{target}' attempted to parse as an expression."),
        };

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

        long value = address.Value;

        // Shift and Address are unsigned. Immediate and offset are the only signed arguments
        bool signed = target is Argument.Immediate or Argument.Offset;

        // Clean integer to fit within argument bit size and match signs.
        long original = 0;
        var cleanStatus = target switch
        {
            Argument.Shift => CleanInteger(ref value, bitCount, signed, out original),
            Argument.Immediate => CleanInteger<short>(ref value, out original),
            Argument.Offset => CleanInteger(ref value, bitCount, signed, out original),
            Argument.Address => CleanInteger(ref value, bitCount, signed, out original),
            Argument.FullImmediate => CleanInteger<int>(ref value, out original),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<int>($"Argument of type '{target}' attempted to parse as an expression."),
        };

        switch (cleanStatus)
        {
            case 0:
                // Integer was already clean
                break;

            case 1:
                // Integer was negative, but needs to be unsigned.
                // Also may have been truncated.
                _logger?.Log(Severity.Warning, LogId.IntegerTruncated, $"Expression '{arg.Print()}' evaluated to signed value {original}," +
                                                                       $" but was cast to unsigned value and truncated to {bitCount}-bits, resulting in {value}.");
                break;
            case 2:
                // Integer was truncated.
                _logger?.Log(Severity.Warning, LogId.IntegerTruncated, $"Expression '{arg.Print()}' evaluated to {original}," +
                                                  $" but was truncated to {bitCount}-bits, resulting in {value}.");
                break;
        }

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
                        _logger?.Log(Severity.Error, LogId.BranchBetweenSections, $"Cannot branch between section.");
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

    private readonly bool TryParseRegister(Token arg, out Register register, RegisterSet set = RegisterSet.GeneralPurpose)
    {
        register = Register.Zero;

        // Check that argument is register argument
        var regStr = arg.Source;
        if (regStr[0] != '$')
        {
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"'{arg}' is not a valid register argument.");
            return false;
        }

        // Get named register from table
        if (!RegistersTable.TryGetRegister(regStr, out register, out RegisterSet parsedSet))
        {
            // Register does not exist in table
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"No register '{arg}' exists.");
            return false;
        }

        // Match register set
        if (parsedSet != RegisterSet.Numbered && parsedSet != set)
        {
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"Register '{arg}' is not parse of register set '{set}'.");
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
    private readonly bool SplitAddressOffset(ReadOnlySpan<Token> arg, out ReadOnlySpan<Token> offset, [NotNullWhen(true)] out Token? register)
    {
        var original = arg;
        register = null;
        offset = arg;

        // Find parenthesis start and end
        // Parenthsis must be matched and contain a single token
        var parIndex = arg.FindNext(TokenType.OpenParenthesis);
        var closeIndex = arg.FindNext(TokenType.CloseParenthesis);
        if (parIndex is -1 || closeIndex is -1 || closeIndex - parIndex != 2)
        {
            _logger?.Log(Severity.Error, LogId.InvalidAddressOffsetArgument, $"Argument '{arg.Print()}' is not a valid address offset.");
            return false;
        }

        // Offset is everything before the parenthesis
        offset = arg[..parIndex];
        arg = arg[(parIndex+1)..];

        // Register is everything between the parenthesis,
        // and must be a single token.
        register = arg[0];
        arg = arg[1..];

        // Parenthesis pair was not found
        // Or contains both an opening and closing parenthesis, but they are not matched.
        // Or there was content following the parenthesis.
        // Or the token inside the parenthesis is not a register.
        if (arg.IsEmpty || register.Type is not TokenType.Register)
        {
            _logger?.Log(Severity.Error, LogId.InvalidAddressOffsetArgument, $"Argument '{original.Print()}' is not a valid address offset.");
            return false;
        }

        return true;
    }

    /// <returns>
    /// 0 if unchanged, 1 if signChanged (maybe also have been truncated), and 2 if truncated.
    /// </returns>
    private static byte CleanInteger(ref long integer, int bitCount, bool signed, out long original)
    {
        // TODO: Support signed truncation
        original = integer;

        // Truncate integer to bit count
        long mask = (1L << bitCount) - 1;
        integer &= mask;

        // Check for sign in unsigned integer
        // TODO: Handle bitCount >= 32
        if (!signed && original < 0 && bitCount < 32)
        {
            // Remove sign from truncated integer
            integer = (uint)integer;
            return 1;
        }

        // Restore sign on signed integers
        if (signed && original < 0)
        {
            integer |= ~mask;
        }

        // Check if truncated lossily
        if ((original & ~mask) != 0 && ~(original | mask) != 0)
            return 2;

        return 0;
    }
    
    /// <returns>
    /// 0 if unchanged, 1 if signChanged (maybe also have been truncated), and 2 if truncated.
    /// </returns>
    private static byte CleanInteger<T>(ref long integer, out long original)
        where T : unmanaged, IBinaryInteger<T>
    {
        original = integer;

        var cast = T.CreateTruncating(original);
        integer = long.CreateSaturating(cast);

        if (long.Sign(original) != T.Sign(cast))
            return 1;

        if (integer != original)
            return 2;

        return 0;
    }
}
