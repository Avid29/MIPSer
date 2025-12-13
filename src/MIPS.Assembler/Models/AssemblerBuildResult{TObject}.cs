// Avishai Dernis 2025

using MIPS.Assembler.Logging;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Models.Modules.Tables;
using System.Collections.Generic;

namespace MIPS.Assembler.Models;

/// <summary>
/// A <see cref="AssemblerResult"/> including the constructed object module.
/// </summary>
/// <typeparam name="TObject">The object module type.</typeparam>
public class AssemblerBuildResult<TObject> : AssemblerBuildResult
    where TObject : IBuildModule
{
    internal AssemblerBuildResult(TObject? objectModule, bool failed, IReadOnlyList<AssemblerLogEntry> logs, IReadOnlyList<SymbolEntry> symbols, Module? module = null)
        : base(failed, logs, symbols, module)
    {
        ObjectModule = objectModule;
    }

    internal AssemblerBuildResult(TObject? objectModule, AssemblerResult childResult)
        : this(objectModule, childResult.Failed, childResult.Logs, childResult.Symbols)
    {
    }

    /// <summary>
    /// Gets the object module result of the assembly.
    /// </summary>
    public TObject? ObjectModule { get; }

    /// <summary>
    /// Gets the object module result of the assembly.
    /// </summary>
    public override IBuildModule? Module => ObjectModule;
}
