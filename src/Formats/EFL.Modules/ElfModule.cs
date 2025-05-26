// Adam Dernis 2025

using EFL.Modules.Models.Header;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;

namespace EFL.Modules;

/// <summary>
/// A fully assembled object module in ELF format
/// </summary>
public class ElfModule : IModule<ElfModule>
{
    /// <inheritdoc/>
    public static ElfModule? Create(ModuleConstructor constructor, AssemblerConfig config, Stream? stream = null)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public static ElfModule? Load(Stream stream)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public ModuleConstructor? Abstract(AssemblerConfig config)
    {
        throw new NotImplementedException();
    }
}
