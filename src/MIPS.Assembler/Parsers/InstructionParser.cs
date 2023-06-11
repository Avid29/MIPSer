// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Helpers;
using MIPS.Assembler.Models.Instructions;
using MIPS.Assembler.Models.Instructions.Enums;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Assembler.Parsers;

/// <summary>
/// A struct for parsing instructions.
/// </summary>
public struct InstructionParser
{
    private InstructionMetadata _meta = default;

    private OperationCode _opCode = default;
    private FunctionCode _funcCode = default;
    private Register _rs = default;
    private Register _rt = default;
    private Register _rd = default;
    private byte _shift = default;
    private short _immediate = default;
    private uint _address = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionParser"/> struct.
    /// </summary>
    public InstructionParser()
    {
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
        if (!Tables.TryGetInstruction(name, out _meta))
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
                ParseShiftArg(arg);
                break;
            case Argument.Immediate:
                ParseImmediateArg(arg);
                break;
            case Argument.Address:
                ParseAddressArg(arg);
                break;
            case Argument.AddressOffset:
                ParseAddressOffsetArg(arg);
                break;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException($"Argument type '{type}' is not in range.");
                break;
        }
    }

    private void ParseRegisterArg(string arg, Argument type)
    {
        // Get reference to selected register argument
        ref Register reg = ref _rs;
        switch (type)
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

    private void ParseShiftArg(string arg)
    {
        // TODO: Expressions and symbols

        // TODO: Handle exceptions
        _shift = byte.Parse(arg);
    }

    private void ParseImmediateArg(string arg)
    {
        // TODO: Expressions and symbols

        // TODO: Handle exceptions
        _immediate = short.Parse(arg);
    }

    private void ParseAddressArg(string arg)
    {
        // TODO: Expressions and symbols

        // TODO: Handle exceptions
        _address = uint.Parse(arg);
    }

    private void ParseAddressOffsetArg(string arg)
    {
        // TODO: Expressions and symbols

        // TODO: Exception handling
        int regStart = arg.IndexOf('(');
        int regEnd = arg.IndexOf(')');

        // Parse offset component
        string offsetStr = arg[..regStart];
        _ = short.TryParse(offsetStr, out _immediate);

        // Parse register component
        string regStr = arg[(regStart + 1)..regEnd];
        // AddressOffset register is always $rs
        _ = TryParseRegister(regStr, out _rs);
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
        if (!Tables.TryGetRegister(name[1..], out register))
        {
            // TODO: Log error
            ThrowHelper.ThrowInvalidDataException($"{name} is not a valid register.");
            return false;
        }

        return true;
    }
}
