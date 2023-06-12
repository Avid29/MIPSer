// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Helpers;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Models.Instructions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// A struct for parsing instructions.
/// </summary>
public struct InstructionParser
{
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
    public InstructionParser()
    {
        _expParser = new ExpressionParser(new IntegerEvaluator());
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
    /// Parses an instruction from a name and a list of arguments.
    /// </summary>
    /// <param name="name">The instruction name.</param>
    /// <param name="args">The instruction arguments.</param>
    /// <returns>An <see cref="Instruction"/> object.</returns>
    public Instruction Parse(string name, string[] args)
    {
        // Get instruction metadata from name
        if (!ConstantTables.TryGetInstruction(name, out _meta))
        {
            // TODO: Log error
            ThrowHelper.ThrowInvalidDataException($"Instruction named '{name}' could not be found.");
        }

        // Assert proper argument count for instruction
        if (args.Length != _meta.ArgumentPattern.Length)
        {
            // TODO: Log error
            ThrowHelper.ThrowArgumentException($"Instruction '{name}' had {args.Length} arguments instead of {_meta.ArgumentPattern.Length}.");
        }

        // Assign op code and function code
        _opCode = _meta.OpCode;
        _funcCode = _meta.FuncCode;

        // Parse argument data according to pattern
        var pattern = _meta.ArgumentPattern;
        for (int i = 0; i < args.Length; i++)
        {
            ParseArg(args[i], pattern[i]);
        }

        // Create an instruction from its components based on the instruction type
        var instruction = _meta.Type switch
        {
            InstructionType.R => Instruction.Create(_funcCode, _rs, _rt, _rd, _shift),
            InstructionType.I => Instruction.Create(_opCode, _rs, _rt, _immediate),
            InstructionType.J => Instruction.Create(_opCode, _address),
            _ => ThrowHelper.ThrowInvalidDataException<Instruction>(""),
        };

        return instruction;
    }

    private void ParseArg(string arg, Argument type)
    {
        switch (type)
        {
            case Argument.RS:
            case Argument.RT:
            case Argument.RD:
                ParseRegisterArg(arg, type);
                break;
            case Argument.Shift:
            case Argument.Immediate:
            case Argument.Address:
                ParseExpressionArg(arg, type);
                break;
            case Argument.AddressOffset:
                ParseAddressOffsetArg(arg);
                break;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException($"Argument type '{type}' is not in range.");
                break;
        }
    }

    /// <summary>
    /// Parses an argument as a register and assigns it to the target component.
    /// </summary>
    private void ParseRegisterArg(string arg, Argument target)
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
        }

        if (!TryParseRegister(arg, out var register))
        {
            // TODO: Log error
        }

        // Cache register as appropriate argument type
        reg = register;
    }

    /// <summary>
    /// Parses an argument as an expression and assigns it to the target component
    /// </summary>
    private void ParseExpressionArg(string arg, Argument target)
    {
        if (!_expParser.TryParse(arg, out var value))
        {
            // TODO: Log error
        }

        switch (target)
        {
            case Argument.Shift:
                _shift = (byte)value;
                break;
            case Argument.Immediate:
                _immediate = (short)value;
                break;
            case Argument.Address:
                _address = (uint)value;
                break;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException($"Argument '{arg}' of type '{target}' was misevaluated as an expression.");
                break;
        }
    }

    /// <summary>
    /// Parses an argument as an address offset, assigning its components to immediate and $rs.
    /// </summary>
    private void ParseAddressOffsetArg(string arg)
    {
        // TODO: Expressions and symbols
        
        // TODO: Exception handling
        int regStart = arg.IndexOf('(');
        int regEnd = arg.IndexOf(')');
        string offsetStr = arg[..regStart];
        string regStr = arg[(regStart + 1)..regEnd];

        // NOTE: Be careful about forwards to other parse functions when implementing 
        // error logging. $rs and immediate errors might be inappropriately logged for
        // address offset arguments.
        
        // Parse offset component into immediate
        ParseExpressionArg(offsetStr, Argument.Immediate);

        // Parse register component into $rs
        ParseRegisterArg(regStr, Argument.RS);
    }

    private static bool TryParseRegister(string name, out Register register)
    {
        // Trim
        name = name.Trim();

        // Check that argument is register argument
        if (name[0] != '$')
        {
            // TODO: Log error
            ThrowHelper.ThrowArgumentException($"Expected register argument. Found '{name}'");
            register = Register.Zero;
            return false;
        }

        // Get register from table
        if (!ConstantTables.TryGetRegister(name[1..], out register))
        {
            // TODO: Log error
            ThrowHelper.ThrowInvalidDataException($"{name} is not a valid register.");
            return false;
        }

        return true;
    }
}
