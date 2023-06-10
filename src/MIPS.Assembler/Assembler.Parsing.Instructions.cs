// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Helpers;
using MIPS.Assembler.Models.Instructions.Enums;
using MIPS.Helpers.Instructions;
using MIPS.Models.Instructions;
using MIPS.Models.Instructions.Enums;

namespace MIPS.Assembler;

public partial class Assembler
{
    private unsafe bool ParseInstruction(string line, int lineNum)
    {
        int instrNameEnd = line.IndexOf(' ');
        Guard.IsNotEqualTo(instrNameEnd, -1);

        string instrName = line[..instrNameEnd];
        if(!Tables.TryGetInstruction(instrName, out var meta))
            ThrowHelper.ThrowInvalidDataException($"Instruction named '{instrName}' on line {lineNum} could not be found.");

        string[] args = line[instrNameEnd..].Trim().Split(',');
        Guard.IsEqualTo(args.Length, meta.ArgumentPattern.Length);

        var opCode = meta.OpCode;
        var funcCode = meta.FuncCode;
        var rs = Register.Zero;
        var rt = Register.Zero;
        var rd = Register.Zero;
        byte shift = 0;
        ushort immediate = 0;
        uint address = 0;

        // Parse by pattern
        var pattern = meta.ArgumentPattern;
        for (int i = 0; i < pattern.Length; i++)
        {
            switch (pattern[i])
            {
                // Register arguments
                case Argument.RS:
                case Argument.RT:
                case Argument.RD:

                    // Get pointer to selected register argument to assign to
                    Register* regPtr = pattern[i] switch
                    {
                        Argument.RS => &rs,
                        Argument.RT => &rt,
                        Argument.RD => &rd,
                        _ => &rs,
                    };

                    // Parse register and assign to selected register argument
                    _ = TryParseRegister(args[i], out var reg);
                    *regPtr = reg;
                    break;

                // TODO: Macros

                // Shift
                case Argument.Shift:
                    _ = byte.TryParse(args[i], out shift);
                    break;

                // Immediate
                case Argument.Immediate:
                    _ = ushort.TryParse(args[i], out immediate);
                    break;

                // Address
                case Argument.Address:
                    _ = uint.TryParse(args[i], out address);
                    break;

                // TODO: AddressOffset
            }
        }

        // Create an instruction from its components based on the instruction type
        var instruction = meta.Type switch
        {
            InstructionType.R => Instruction.Create(funcCode, rs, rt, rd, shift),
            InstructionType.I => Instruction.Create(opCode, rs, rt, immediate),
            InstructionType.J => Instruction.Create(opCode, address),
            _ => ThrowHelper.ThrowInvalidDataException<Instruction>(""),
        };

        Append(instruction);
        return true;
    }

    private bool TryParseRegister(string regArg, out Register register)
    {
        regArg = regArg.Trim();
        Guard.IsEqualTo(regArg[0], '$');
        var regName = regArg[1..];

        if (!Tables.TryGetRegister(regName, out register))
        {
            ThrowHelper.ThrowInvalidDataException($"{regArg} is not a valid register.");
            return false;
        }

        return true;
    }
}
