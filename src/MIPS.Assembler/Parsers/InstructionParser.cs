// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Helpers;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Tokenization;
using MIPS.Assembler.Tokenization.Enums;
using MIPS.Extensions.MIPS.Models.Instructions;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// A struct for parsing instructions.
/// </summary>
public struct InstructionParser
{
    private readonly ModuleConstruction? _context;
    private readonly ILogger? _logger;

    private InstructionMetadata _meta;

    private OperationCode _opCode;
    private FunctionCode _funcCode;
    private Register _rs;
    private Register _rt;
    private Register _rd;
    private byte _shift;
    private int _immediate;
    private uint _address;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionParser"/> struct.
    /// </summary>
    public InstructionParser() : this(null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionParser"/> struct.
    /// </summary>
    public InstructionParser(ModuleConstruction? content, ILogger? logger)
    {
        _context = content;
        _logger = logger;
        _meta = default;
        _opCode = default;
        _funcCode = default;
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
    /// <param name="name">The instruction name.</param>
    /// <param name="args">The instruction arguments.</param>
    /// <param name="instruction">The <see cref="Instruction"/>.</param>
    /// <param name="pseudo">The <see cref="PseudoInstruction"/>.</param>
    /// <param name="relSymbol">The relocatable symbol referenced on this line. Or null if none.</param>
    /// <returns>Whether or not an instruction was parsed.</returns>
    public bool TryParse(Token name, Span<Token> args, out Instruction instruction, out PseudoInstruction pseudo, out string? relSymbol)
    {
        relSymbol = null;
        instruction = default;
        pseudo = default;

        // Get instruction metadata from name
        if (!ConstantTables.TryGetInstruction(name.Source, out _meta))
        {
            _logger?.Log(Severity.Error, LogId.InvalidInstructionName, $"No instruction named '{name}'.");
            return false;
        }

        // Assign op code and function code
        _opCode = _meta.OpCode;
        _funcCode = _meta.FuncCode;

        // Parse argument data according to pattern
        Argument[] pattern = _meta.ArgumentPattern;

        int argC;
        for (argC = 0; !args.IsEmpty; argC++)
        {
            // Assert proper argument count for instruction
            if (argC >= pattern.Length)
            {
                _logger?.Log(Severity.Error, LogId.InvalidInstructionArgCount, $"Instruction '{name}' has too many arguments! Found {argC + 1} arguments when expecting {_meta.ArgumentPattern.Length}.");
                return false;
            }

            // Split out next arg
            args = args.SplitAtNext(TokenType.Comma, out var arg, out _);
            TryParseArg(arg, pattern[argC], out relSymbol);
        }

        // Assert proper argument count for instruction
        if (argC < pattern.Length)
        {
            _logger?.Log(Severity.Error, LogId.InvalidInstructionArgCount, $"Instruction '{name}' doesn't have enough arguments. Found {argC} arguments when expecting {_meta.ArgumentPattern.Length}.");
            return false;
        }

        // Handle the pseudo-instruction condition
        if (_meta.IsPseudoInstruction)
        {
            var pseudoOp = _meta.PseudoOp;
            pseudo = pseudoOp switch
            {
                PseudoOp.BranchOnLessThan => new PseudoInstruction(pseudoOp) { RS = _rs, RT = _rt, Immediate = _immediate },
                PseudoOp.LoadImmediate => new PseudoInstruction(pseudoOp) { RT = _rt, Immediate = _immediate },
                PseudoOp.AbsoluteValue => new PseudoInstruction(pseudoOp) { RS = _rs, RT = _rt },
                PseudoOp.Move => new PseudoInstruction(pseudoOp) { RS = _rs, RT = _rt },
                PseudoOp.LoadAddress => new PseudoInstruction(pseudoOp) { RT = _rt, Address = _address },
                PseudoOp.SetGreaterThanOrEqual => new PseudoInstruction(pseudoOp) { RS = _rs, RT = _rt, RD = _rd },
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<PseudoInstruction>(),
            };
            return true;
        }

        // Create the instruction from its components based on the instruction type
        instruction = _meta.Type switch
        {
            InstructionType.R when _opCode is OperationCode.BranchConditional => Instruction.Create(_meta.BranchCode, _rs, (short)_immediate),
            InstructionType.R => Instruction.Create(_funcCode, _rs, _rt, _rd, _shift),
            InstructionType.I => Instruction.Create(_opCode, _rs, _rt, (short)_immediate),
            InstructionType.J => Instruction.Create(_opCode, _address),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<Instruction>($"Invalid instruction type '{_meta.Type}'."),
        };

        // Check for write back to zero register
        // Give a warning if not an explicit nop operation
        // TODO: Check on pseudo-instructions
        if (instruction.GetWritebackRegister() is Register.Zero && name.Source != "nop")
        {
            _logger?.Log(Severity.Message, LogId.ZeroRegWriteBack, "This instruction writes to $zero.");
        }

        return true;
    }

    private bool TryParseArg(Span<Token> arg, Argument type, out string? relSymbol)
    {
        relSymbol = null;

        return type switch
        {
            Argument.RS or Argument.RT or Argument.RD =>TryParseRegisterArg(arg[0], type),
            Argument.Shift or Argument.Immediate or Argument.FullImmediate or Argument.Offset or Argument.Address => TryParseExpressionArg(arg, type, out relSymbol),
            Argument.AddressOffset => TryParseAddressOffsetArg(arg, out relSymbol),
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
        switch (target)
        {
            case Argument.RS:
                reg = ref _rs;
                break;
            case Argument.RT:
                reg = ref _rt;
                break;
            case Argument.RD:
                reg = ref _rd;
                break;

            // Invalid target type
            default:
                // TODO: improve message
                return ThrowHelper.ThrowArgumentOutOfRangeException<bool>($"Argument of type '{target}' attempted to parse as a register.");
        }

        if (!TryParseRegister(arg, out var register))
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
    private bool TryParseExpressionArg(Span<Token> arg, Argument target, out string? relSymbol)
    {
        var parser = new ExpressionParser(_context, _logger);

        // Attempt to parse expression
        if (!parser.TryParse(arg, out var address, out relSymbol))
            return false;

        // NOTE: Casting might truncate the value to fit the bit size.
        // This is the desired behavior, but when logging errors this
        // should be handled explicitly and drop an assembler warning.

        // Determine the bits allowed by the 
        int bitCount = target switch
        {
            Argument.Shift => 5,
            Argument.Immediate => 16,
            Argument.Address => 26,
            Argument.FullImmediate => 32,
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<int>($"Argument of type '{target}' attempted to parse as an expression."),
        };

        if (address.IsRelocatable && target is Argument.Shift)
        {
            _logger?.Log(Severity.Error, LogId.RelocatableReferenceInShift, "Shift amount argument cannot reference relocatable symbols.");
            return false;
        }

        long value = address.Value;

        // Shift and Address are unsigned. Immediate is the only signed argument
        bool signed = target is Argument.Immediate;

        // Clean integer to fit within argument bit size and match signs.
        switch (CleanInteger(ref value, bitCount, signed, out var original))
        {
            case 0:
                // Integer was already clean
                break;

            case 1:
                // Integer was negative, but needs to be unsigned.
                // Also may have been truncated.
                // TODO: Argument printing
                _logger?.Log(Severity.Warning, LogId.IntegerTruncated, $"Expression '' evaluated to signed value {original}," +
                                                                       $" but was cast to unsigned value and truncated to {bitCount}-bits, resulting in {value}.");
                break;
            case 2:
                // Integer was truncated.
                // TODO: Argument printing
                _logger?.Log(Severity.Warning, LogId.IntegerTruncated, $"Expression '' evaluated to {original}," +
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
            case Argument.Offset:
                // TODO: Make relative to current position.
                _immediate = (short)value;
                return true;
            case Argument.Address:
                _address = (uint)value;
                return true;

            // Invalid target type
            default:
                // TODO: Argument printing
                return ThrowHelper.ThrowArgumentOutOfRangeException<bool>($"Argument '' of type '{target}' attempted to parse as an expression.");
        }
    }

    /// <summary>
    /// Parses an argument as an address offset, assigning its components to immediate and $rs.
    /// </summary>
    private bool TryParseAddressOffsetArg(Span<Token> arg, out string? relSymbol)
    {
        relSymbol = null;

        // NOTE: Be careful about forwards to other parse functions with regards to 
        // error logging. Address offset argument errors might be inappropriately logged.


        // Split the string into an offset and a register, return false if failed
        if (!TokenizeAddressOffset(arg, out var offsetStr, out var regStr))
            return false;

        // Try parse offset component into immediate, return false if failed
        if (!TryParseExpressionArg(offsetStr, Argument.Immediate, out relSymbol))
            return false;

        // Parse register component into $rs, return false if failed
        if (!TryParseRegisterArg(regStr, Argument.RS))
            return false;

        return true;
    }

    private readonly bool TryParseRegister(Token arg, out Register register)
    {
        register = Register.Zero;

        // Check that argument is register argument
        var regStr = arg.Source;
        if (regStr[0] != '$')
        {
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"'{arg}' is not a valid register argument.");
            return false;
        }

        // Get register from table
        if (!ConstantTables.TryGetRegister(regStr[1..], out register))
        {
            // Register does not exist in table
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"No register '{arg}' exists.");
            return false;
        }

        return true;
    }

    /// <remarks>
    /// Upon return offset and register do not need to be valid offset and register strings.
    /// The register is just the component in parenthesis. The offset is just the component before the parenthesis.
    /// Nothing may follow the parenthesis.
    /// </remarks>
    private bool TokenizeAddressOffset(Span<Token> arg, out Span<Token> offset, out Token register)
    {
        // Find parenthesis start and end
        // Parenthesis wrap the register
        arg = arg.SplitAtNext(TokenType.OpenParenthesis, out offset, out _);
        register = arg[0];

        // Parenthesis pair was not found
        // Or contains both an opening and closing parenthesis, but they are not matched.
        // Or there was content following the parenthesis 
        if (arg.IsEmpty)
        {
            // TODO: Argument printing
            _logger?.Log(Severity.Error, LogId.InvalidAddressOffsetArgument, $"Argument '' is not a valid address offset.");
            return false;
        }

        if (register.Type is not TokenType.Register)
        {
            // TODO: Argument printing
            _logger?.Log(Severity.Error, LogId.InvalidAddressOffsetArgument, $"Argument '' is not a valid address offset.");
            return false;
        }

        return true;
    }

    /// <returns>
    /// 0 if unchanged, 1 if signChanged (maybe also have been truncated), and 2 if truncated.
    /// </returns>
    private static int CleanInteger(ref long integer, int bitCount, bool signed, out long original)
    {
        original = integer;

        // Truncate integer to bit count
        long mask = (1L << bitCount) - 1;
        integer &= mask;

        // Check for sign change
        // TODO: Handle bitCount >= 32
        if (!signed && original < 0 && bitCount < 32)
        {
            // Remove sign from truncated integer
            integer = (uint)integer;
            return 1;
        }

        // Check if truncated
        if (integer != original)
            return 2;

        return 0;
    }
}
