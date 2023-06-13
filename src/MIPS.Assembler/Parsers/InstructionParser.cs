// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Helpers;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Logging.Enum;
using MIPS.Assembler.Models.Construction;
using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// A struct for parsing instructions.
/// </summary>
public struct InstructionParser
{
    private ILogger? _logger;
    private ExpressionParser _expParser;

    private InstructionMetadata _meta;

    private OperationCode _opCode;
    private FunctionCode _funcCode;
    private Register _rs;
    private Register _rt;
    private Register _rd;
    private byte _shift;
    private short _immediate;
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
    public InstructionParser(ObjectModuleConstructor? obj, ILogger? logger)
    {
        _logger = logger;
        _expParser = new ExpressionParser(obj, logger);
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
    /// <returns>Whether or not an instruction was parsed.</returns>
    public bool TryParse(string name, string[] args, out Instruction instruction)
    {
        instruction = default;

        // Get instruction metadata from name
        if (!ConstantTables.TryGetInstruction(name, out _meta))
        {
            _logger?.Log(Severity.Error, LogId.InvalidInstructionName, $"Instruction named '{name}' could not be found.");
            return false;
        }

        // Assert proper argument count for instruction
        if (args.Length != _meta.ArgumentPattern.Length)
        {
            _logger?.Log(Severity.Error, LogId.InvalidInstructionArgCount, $"Instruction '{name}' had {args.Length} arguments instead of {_meta.ArgumentPattern.Length}.");
            return false;
        }

        // Assign op code and function code
        _opCode = _meta.OpCode;
        _funcCode = _meta.FuncCode;

        // Parse argument data according to pattern
        Argument[] pattern = _meta.ArgumentPattern;
        for (int i = 0; i < args.Length; i++)
            TryParseArg(args[i], pattern[i]);

        // Create the instruction from its components based on the instruction type
        instruction = _meta.Type switch
        {
            InstructionType.R => Instruction.Create(_funcCode, _rs, _rt, _rd, _shift),
            InstructionType.I => Instruction.Create(_opCode, _rs, _rt, _immediate),
            InstructionType.J => Instruction.Create(_opCode, _address),
            _ => ThrowHelper.ThrowInvalidDataException<Instruction>(""),
        };

        return true;
    }

    private bool TryParseArg(string arg, Argument type)
    {
        switch (type)
        {
            // Register type argument
            case Argument.RS:
            case Argument.RT:
            case Argument.RD:
                return TryParseRegisterArg(arg, type);
            // Immediate type argument
            case Argument.Shift:
            case Argument.Immediate:
            case Argument.Address:
                return TryParseExpressionArg(arg, type);
            // Address offset type argument
            case Argument.AddressOffset:
                return TryParseAddressOffsetArg(arg);

            // Invalid type
            default:
                _logger?.Log(Severity.Error, LogId.InstructionIncorrectParsingMethod, $"Argument '{arg}' of type '{type}' is not within parsable type range.");
                return false;
        }
    }

    /// <summary>
    /// Parses an argument as a register and assigns it to the target component.
    /// </summary>
    private bool TryParseRegisterArg(string arg, Argument target)
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
                _logger?.Log(Severity.Error, LogId.InstructionIncorrectParsingMethod, $"Argument '{arg}' of type '{target}' attempted to parse as a register.");
                return false;
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
    private bool TryParseExpressionArg(string arg, Argument target)
    {
        if (!_expParser.TryParse(arg, out var value))
        {
            // TODO: Log error and remove exception

            ThrowHelper.ThrowArgumentException($"Argument '{arg}' of type '{target}' was not a valid expression.");
        }

        // NOTE: Casting might truncate the value to fit the bit size.
        // This is the desired behavior, but when logging errors this
        // should be handled explicitly and drop an assembler warning.
        
        // Assign to appropriate instruction argument
        switch (target)
        {
            case Argument.Shift:
                _shift = (byte)value;
                return true;
            case Argument.Immediate:
                _immediate = (short)value;
                return true;
            case Argument.Address:
                _address = (uint)value;
                return true;
            default:
                _logger?.Log(Severity.Error, LogId.InstructionIncorrectParsingMethod, $"Argument '{arg}' of type '{target}' attempted to parse as an expression.");
                return false;
        }
    }

    /// <summary>
    /// Parses an argument as an address offset, assigning its components to immediate and $rs.
    /// </summary>
    private bool TryParseAddressOffsetArg(string arg)
    {
        // NOTE: Be careful about forwards to other parse functions with regards to 
        // error logging. Address offset argument errors might be inappropriately logged.


        // Split the string into an offset and a register, return false if failed
        if (!TokenizeAddressOffset(arg, out var offsetStr, out var regStr))
            return false;
        
        // Try parse offset component into immediate, return false if failed
        if (!TryParseExpressionArg(offsetStr, Argument.Immediate))
            return false;

        // Parse register component into $rs, return false if failed
        if(!TryParseRegisterArg(regStr, Argument.RS))
            return false;

        return true;
    }

    private bool TryParseRegister(string name, out Register register)
    {
        register = Register.Zero;

        // Trim whitespace from register string
        name = name.Trim();

        // Check that argument is register argument
        if (name[0] != '$')
        {
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"Expected register argument. Found '{name}'");
            return false;
        }

        // Get register from table
        if (!ConstantTables.TryGetRegister(name[1..], out register))
        {
            // Register does not exist in table
            _logger?.Log(Severity.Error, LogId.InvalidRegisterArgument, $"{name} is not a valid register.");
            return false;
        }

        return true;
    }

    /// <remarks>
    /// Upon return offset and register do not need to be valid offset and register strings.
    /// The register is just the component in parenthesis. The offset is just the component before the parenthesis.
    /// Nothing may follow the parenthesis.
    /// </remarks>
    private bool TokenizeAddressOffset(string arg, out string offset, out string register)
    {
        // Trim whitespace 
        arg = arg.Trim();

        offset = string.Empty;
        register = string.Empty;

        // Find parenthesis start and end
        // Parenthesis wrap the register
        int regStart = arg.IndexOf('(');
        int regEnd = arg.IndexOf(')');

        // Either end of the parenthesis were not found
        // Or contains both an opening and closing parenthesis, but they are not matched.
        // Or there was content following the register
        if (regStart == -1 || regEnd == -1 || regStart > regEnd || regEnd != arg.Length - 1)
        {
            ThrowHelper.ThrowArgumentException($"Argument '{arg}' is not a valid address offset.");
            return false;
        }

        // Split argument into offset and register components
        // Argument and offset validity will be assessed outside of tokenization
        offset = arg[..regStart];
        register = arg[(regStart + 1)..regEnd];

        return true;
    }
}
