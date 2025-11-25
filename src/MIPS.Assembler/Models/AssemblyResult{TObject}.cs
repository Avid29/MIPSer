// Avishai Dernis 2025

using MIPS.Assembler.Logging;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Models.Modules.Tables;
using System.Collections.Generic;

namespace MIPS.Assembler.Models;

/// <summary>
/// A <see cref="AssemblyResult"/> including the constructed object module.
/// </summary>
/// <typeparam name="TObject">The object module type.</typeparam>
public class AssemblyResult<TObject> : AssemblyResult
    where TObject : IBuildModule
{
    internal AssemblyResult(TObject? objectModule, bool failed, IReadOnlyList<AssemblerLog> logs, IReadOnlyList<SymbolEntry> symbols)
        : base(failed, logs, symbols)
    {
        ObjectModule = objectModule;
    }

    internal AssemblyResult(TObject? objectModule, AssemblyResult childResult)
        : this(objectModule, childResult.Failed, childResult.Logs, childResult.Symbols)
    {
    }

    /// <summary>
    /// Gets the object module result of the assembly.
    /// </summary>
    public TObject? ObjectModule { get; }
}
