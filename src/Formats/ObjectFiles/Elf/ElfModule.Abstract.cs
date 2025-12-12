// Avishai Dernis 2025

using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules;

namespace ObjectFiles.Elf;

public partial class ElfModule
{
    /// <inheritdoc/>
    public Module? Abstract(AssemblerConfig config)
    {
        throw new System.NotImplementedException();
    }
}
