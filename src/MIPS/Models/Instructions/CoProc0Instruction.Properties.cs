// Adam Dernis 2025

using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using MIPS.Models.Instructions.Enums.SpecialFunctions.CoProc0;

namespace MIPS.Models.Instructions;

public partial struct CoProc0Instruction
{
    /// <summary>
    /// Gets the instruction's operation code.
    /// </summary>
    public OperationCode OpCode
    { 
        readonly get => _inst.OpCode;
        internal set => _inst.OpCode = value;
    }

    /// <summary>
    /// Gets the instruction's RS function code.
    /// </summary>
    public CoProc0RSCode CoProc0RSCode
    { 
        readonly get => (CoProc0RSCode)_inst.RS;
        internal set => _inst.RS = (Register)value;
    }

    /// <summary>
    /// Gets the instruction's RT Register 
    /// </summary>
    public Register RT
    { 
        readonly get => _inst.RT;
        internal set => _inst.RT = value;
    }
    
    /// <summary>
    /// Gets the instruction's RD Register 
    /// </summary>
    public Register RD
    { 
        readonly get => _inst.RD;
        internal set => _inst.RD = value;
    }
    
    /// <summary>
    /// Gets the instruction's coproc0 function code.
    /// </summary>
    /// <remarks>
    /// Instruction may or may not have coproc0 function code.
    /// </remarks>
    public Co0FuncCode Co0FuncCode
    { 
        readonly get => (Co0FuncCode)_inst.FuncCode;
        internal set => _inst.FuncCode = (FunctionCode)value;
    }

    /// <summary>
    /// Gets the instruction's mfmc0 function code.
    /// </summary>
    /// <remarks>
    /// Instruction may or may not have coproc0 function code.
    /// </remarks>
    public MFMC0FuncCode MFMC0FuncCode
    {
        readonly get => (MFMC0FuncCode)_inst.FuncCode;
        internal set => _inst.FuncCode = (FunctionCode)value;
    }
}
