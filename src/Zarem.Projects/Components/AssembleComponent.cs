// Avishai Dernis 2026

using CommunityToolkit.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Assembler.Logging;
using Zarem.Assembler.Models;
using Zarem.Components.Interfaces;
using Zarem.Models.Files;

namespace Zarem.Components;

/// <summary>
/// A component of a <see cref="Project"/> class for assembling assembly code.
/// </summary>
/// <typeparam name="TAssembler"></typeparam>
/// <typeparam name="TConfig"></typeparam>
public class AssembleComponent<TAssembler, TConfig> : IAssembleComponent
    where TAssembler : IAssembler<TConfig>
    where TConfig : AssemblerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssembleComponent{TAssembler, TConfig}"/> class.
    /// </summary>
    public AssembleComponent(TConfig config)
    {
        Config = config;
    }

    /// <inheritdoc/>
    public TConfig Config { get; }

    AssemblerConfig IAssembleComponent.Config => Config;

    /// <inheritdoc/>
    public async Task<AssemblerResult?> AssembleFileAsync(SourceFile file, bool rebuild = true, Logger? logger = null)
    {
        // Skip if not dirty and not rebuilding
        if (!(file.IsDirty || !file.ObjectFile.Exists) && !rebuild)
            return null;

        Guard.IsNotNull(Config);

        using var stream = File.OpenRead(file.FullPath);
        var result = await TAssembler.AssembleAsync(stream, file.Name, Config, logger);
        return result;
    }
}
