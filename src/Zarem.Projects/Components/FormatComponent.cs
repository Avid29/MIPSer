// Avishai Dernis 2026

using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.Models.Modules;
using Zarem.Components.Interfaces;
using Zarem.Config;
using Zarem.Models.Modules;

namespace Zarem.Components;

/// <summary>
/// A component of the <see cref="Project"/> which handles assembling.
/// </summary>
/// <typeparam name="TModule">The target module format's type.</typeparam>
/// <typeparam name="TConfig">The type for the format's config.</typeparam>
public class FormatComponent<TModule, TConfig> : IFormatComponent
    where TModule : IBuildModule<TModule, TConfig>
    where TConfig : FormatConfig, new()
{
    /// <summary>
    /// Initialize a new instance of the <see cref="FormatComponent{TModule, TConfig}"/> class.
    /// </summary>
    public FormatComponent(TConfig config)
    {
        Config = config;
    }

    /// <inheritdoc/>
    public TConfig Config { get; }

    FormatConfig IFormatComponent.Config => Config;

    /// <inheritdoc/>
    public async Task<bool> TryExportAsync(Module module, string path)
    {
        var export = TModule.Create(module, Config);
        if (export is null)
            return false;

        using var outstream = File.Open(path, FileMode.OpenOrCreate);
        await export.SaveAsync(outstream);
        return true;
    }
}
