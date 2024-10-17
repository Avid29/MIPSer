// Adam Dernis 2024

using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using System.Collections.Generic;

namespace MIPS.Assembler.Helpers.Tables;

public static partial class InstructionsTable
{
    // Co-Processor
    private static readonly Argument[] LoadCoprocessor = [];    // TODO:
    private static readonly Argument[] StoreCoprocessor = [];   // TODO:

    /// <summary>
    /// Co-processor instruction table.
    /// </summary>
    private static readonly Dictionary<string, InstructionMetadata> _coProcInstructionTable = new()
    {
        { "lwc1",   new("lwc1",     OperationCode.LoadWordCoprocessor1,     LoadCoprocessor) },
        { "lwc2",   new("lwc2",     OperationCode.LoadWordCoprocessor2,     LoadCoprocessor) },
        { "lwc3",   new("lwc3",     OperationCode.LoadWordCoprocessor3,     LoadCoprocessor,    Version.MipsItoII) },
        
        { "swc1",   new("swc1",     OperationCode.StoreWordCoprocessor1,    StoreCoprocessor) },
        { "swc2",   new("swc2",     OperationCode.StoreWordCoprocessor2,    StoreCoprocessor) },
        { "swc3",   new("swc3",     OperationCode.StoreWordCoprocessor3,    StoreCoprocessor,   Version.MipsItoII) },

        //{ "ll",     new("ll",       OperationCode.LoadLinkedWord,           )},
        //{ "sc",     new("sc",       OperationCode.LoadLinkedWord,           )},
    };
}
