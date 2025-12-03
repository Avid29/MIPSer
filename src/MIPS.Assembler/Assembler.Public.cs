// Avishai Dernis 2025

using MIPS.Assembler.Models;
using MIPS.Assembler.Models.Config;
using MIPS.Assembler.Models.Modules.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace MIPS.Assembler;

public partial class Assembler
{
    /// <summary>
    /// Assembles a string.
    /// </summary>
    public static async Task<AssemblyResult> AssembleAsync(string str, string? filename, AssemblerConfig config)
    {
        using var reader = new StringReader(str);
        var assembler = await AssembleAsync(reader, filename, config);
        return new AssemblyResult(assembler.Failed, assembler.Logs, assembler.Symbols);
    }
    
    /// <summary>
    /// Assembles a string.
    /// </summary>
    public static async Task<AssemblyResult<T>> AssembleAsync<T, TConfig>(string str, string? filename, TConfig config, Stream? outStream = null)
        where T : IBuildModule<T, TConfig>
        where TConfig : AssemblerConfig
    {
        using var reader = new StringReader(str);
        var assembler = await AssembleAsync(reader, filename, config);
        var obj = T.Create(assembler._module, config, outStream);
        return new AssemblyResult<T>(obj, assembler.Failed, assembler.Logs, assembler.Symbols);
    }

    /// <summary>
    /// Assembles a stream.
    /// </summary>
    public static async Task<AssemblyResult> AssembleAsync(Stream stream, string? filename, AssemblerConfig config)
    {
        using var reader = new StreamReader(stream);
        var assembler = await AssembleAsync(reader, filename, config);
        return new AssemblyResult(assembler.Failed, assembler.Logs, assembler.Symbols);
    }

    /// <summary>
    /// Assembles a stream.
    /// </summary>
    public static async Task<AssemblyResult<T>> AssembleAsync<T, TConfig>(Stream stream, string? filename, TConfig config, Stream? outStream = null)
        where T : IBuildModule<T, TConfig>
        where TConfig : AssemblerConfig
    {
        using var reader = new StreamReader(stream);
        var assembler = await AssembleAsync(reader, filename, config);
        var obj = T.Create(assembler._module, config, outStream);
        return new AssemblyResult<T>(obj, assembler.Failed, assembler.Logs, assembler.Symbols);
    }
}
