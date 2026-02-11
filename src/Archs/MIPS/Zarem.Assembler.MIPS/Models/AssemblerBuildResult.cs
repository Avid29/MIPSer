// Avishai Dernis 2025

using Zarem.Assembler.MIPS.Logging;
using Zarem.Assembler.MIPS.Models.Modules;
using Zarem.Assembler.MIPS.Models.Modules.Interfaces;
using Zarem.MIPS.Models.Modules.Tables;
using System.Collections.Generic;

namespace Zarem.Assembler.MIPS.Models
{
    /// <summary>
    /// A base class for an <see cref="AssemblerResult"/> including the constructed object module.
    /// </summary>
    public abstract class AssemblerBuildResult : AssemblerResult
    {
        internal AssemblerBuildResult(bool failed, IReadOnlyList<AssemblerLogEntry> logs, IReadOnlyList<SymbolEntry> symbols, Module? module = null) : base(failed, logs, symbols, module)
        {
        }

        /// <summary>
        /// Gets the object module result of the assembly.
        /// </summary>
        public abstract IBuildModule? Module { get; }
    }
}
