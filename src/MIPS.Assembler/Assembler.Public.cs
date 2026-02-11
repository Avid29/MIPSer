// Avishai Dernis 2025

using MIPS.Assembler.Config;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Modules.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler;

public partial class Assembler
{
    /// <summary>
    /// Assembles a string.
    /// </summary>
    public static async Task<AssemblerResult> AssembleAsync(string str, string? filename, AssemblerConfig config, Logger? logger = null)
    {
        using var reader = new StringReader(str);
        var assembler = await AssembleAsync(reader, filename, config, logger);
        return new AssemblerResult(assembler.Failed, assembler.Logs, assembler.Symbols, assembler._module);
    }
    
    /// <summary>
    /// Assembles a string.
    /// </summary>
    public static async Task<AssemblerBuildResult<TModule>> AssembleAsync<TModule, TConfig>(string str, string? filename, TConfig config, Logger? logger = null)
        where TModule : IBuildModule<TModule>
        where TConfig : AssemblerConfig
    {
        using var reader = new StringReader(str);
        var assembler = await AssembleAsync(reader, filename, config, logger);

        TModule? obj = default;
        if (!assembler.Failed)
        {
            obj = TModule.Create(assembler._module, config);
        }

        return new AssemblerBuildResult<TModule>(obj, assembler.Failed, assembler.Logs, assembler.Symbols, assembler._module);
    }

    /// <summary>
    /// Assembles a stream.
    /// </summary>
    public static async Task<AssemblerResult> AssembleAsync(Stream stream, string? filename, AssemblerConfig config, Logger? logger = null)
    {
        using var reader = new StreamReader(stream);
        var assembler = await AssembleAsync(reader, filename, config, logger);
        return new AssemblerResult(assembler.Failed, assembler.Logs, assembler.Symbols, assembler._module);
    }

    /// <summary>
    /// Assembles a stream.
    /// </summary>
    public static async Task<AssemblerBuildResult<TModule>> AssembleAsync<TModule, TConfig>(Stream stream, string? filename, TConfig config, Logger? logger = null)
        where TModule : IBuildModule<TModule>
        where TConfig : AssemblerConfig
    {
        using var reader = new StreamReader(stream);
        var assembler = await AssembleAsync(reader, filename, config, logger);

        TModule? obj = default;
        if (!assembler.Failed)
        {
            obj = TModule.Create(assembler._module, config);
        }

        return new AssemblerBuildResult<TModule>(obj, assembler.Failed, assembler.Logs, assembler.Symbols, assembler._module);
    }
}
