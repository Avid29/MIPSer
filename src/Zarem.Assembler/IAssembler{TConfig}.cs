// Adam Dernis 2024

using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.Config;
using Zarem.Assembler.Logging;
using Zarem.Assembler.Models;

namespace Zarem.Assembler;

/// <summary>
/// An assembler.
/// </summary>
public interface IAssembler<TConfig> : IAssembler
    where TConfig : AssemblerConfig
{
    /// <summary>
    /// Gets the assembler's configuration.
    /// </summary>
    public TConfig Config { get; }

    /// <summary>
    /// Assembles a stream.
    /// </summary>
    static abstract Task<AssemblerResult> AssembleAsync(Stream stream, string? filename, TConfig config, Logger? logger = null);

    /// <summary>
    /// Assembles a string.
    /// </summary>
    static abstract Task<AssemblerResult> AssembleAsync(string str, string? filename, TConfig config, Logger? logger = null);
}
