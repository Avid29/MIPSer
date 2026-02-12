// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Assembler.Logging;
using Zarem.Assembler.Models;
using Zarem.Emulator;
using Zarem.Emulator.Config;
using Zarem.Models;
using Zarem.Models.Files;

namespace Zarem;

public abstract partial class Project<TAssembler, TEmulator, TAsmConfig, TEmuConfig>
    where TAssembler : IAssembler<TAsmConfig>
    where TEmulator : Emulator<TEmuConfig>
    where TAsmConfig : AssemblerConfig
    where TEmuConfig : EmulatorConfig
{
    /// <inheritdoc/>
    public async Task<BuildResult?> BuildProjectAsync(bool rebuild = false, Logger? logger = null)
    {
        var result = await AssembleFilesAsync(SourceFiles, rebuild, logger);

        // TODO: Link

        return result;
    }

    /// <inheritdoc/>
    public async Task<BuildResult?> AssembleFilesAsync(IEnumerable<SourceFile> files, bool rebuild = true, Logger? logger = null)
    {
        var result = new BuildResult();
        foreach (var file in files)
        {
            var assemblyResult = await AssembleFileAsync(file, rebuild, logger);
            result.Add(file, assemblyResult);
        }

        return result;
    }

    /// <inheritdoc/>
    public void CleanProject() => CleanFiles(SourceFiles);

    /// <inheritdoc/>
    public void CleanFiles(IEnumerable<SourceFile> files)
    {
        foreach (var file in files)
            CleanFile(file);
    }

    private async Task<AssemblerResult?> AssembleFileAsync(SourceFile file, bool rebuild = true, Logger? logger = null)
    {
        // Skip if not dirty and not rebuilding
        if (!(file.IsDirty || !file.ObjectFile.Exists) && !rebuild)
            return null;

        Guard.IsNotNull(Config?.AssemblerConfig);

        try
        {
            using var stream = File.OpenRead(file.FullPath);

            var result = await TAssembler.AssembleAsync(stream, file.Name, Config.AssemblerConfig);

            // Write the object file if the build succeeded
            if (!result.Failed && result.Module is not null)
            {
                // TODO: Select module type by format
                //var module = ElfModule.Create(result.Module, new ElfConfig());

                using var outStream = File.Open(file.ObjectFile.FullPath, FileMode.OpenOrCreate);
                //module?.Save(outStream);
            }

            return result;
        }
        catch
        {
            // TODO: Handle error
            CleanFile(file);
            return null;
        }
    }

    private static bool CleanFile(SourceFile file)
    {
        if (!file.ObjectFile.Exists)
            return false;

        try
        {
            File.Delete(file.ObjectFile.FullPath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
