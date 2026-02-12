// Avishai Dernis 2025

using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.Config;
using Zarem.Assembler.Logging;
using Zarem.Assembler.Models;

namespace Zarem.Assembler;

public partial class MIPSAssembler
{
    /// <summary>
    /// Assembles a string.
    /// </summary>
    public static async Task<AssemblerResult> AssembleAsync(string str, string? filename, MIPSAssemblerConfig config, Logger? logger = null)
    {
        using var reader = new StringReader(str);
        var assembler = await AssembleAsync(reader, filename, config, logger);
        return new AssemblerResult(assembler.Failed, assembler.Logs, assembler.Symbols, assembler._module);
    }

    /// <summary>
    /// Assembles a stream.
    /// </summary>
    public static async Task<AssemblerResult> AssembleAsync(Stream stream, string? filename, MIPSAssemblerConfig config, Logger? logger = null)
    {
        using var reader = new StreamReader(stream);
        var assembler = await AssembleAsync(reader, filename, config, logger);
        return new AssemblerResult(assembler.Failed, assembler.Logs, assembler.Symbols, assembler._module);
    }
}
