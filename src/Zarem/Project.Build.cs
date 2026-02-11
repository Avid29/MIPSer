// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using ObjectFiles.Elf;
using ObjectFiles.Elf.Config;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.MIPS.Logging;
using Zarem.Assembler.MIPS.Models;
using Zarem.Models;
using Zarem.Models.Files;
using Zarem.ObjFormats.RASM;
using Zarem.ObjFormats.RASM.Config;

namespace Zarem;

public partial class Project
{
    /// <summary>
    /// Builds the project.
    /// </summary>
    public async Task<BuildResult?> BuildProjectAsync(bool rebuild = false, Logger? logger = null)
    {
        var result = await AssembleFilesAsync(SourceFiles, rebuild, logger);

        // TODO: Link

        return result;
    }

    /// <summary>
    /// Assembles a list of files.
    /// </summary>
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

    /// <summary>
    /// Cleans all files in the project.
    /// </summary>
    public void CleanProject() => CleanFiles(SourceFiles);

    /// <summary>
    /// Cleans a list of source files.
    /// </summary>
    /// <param name="files">The files to clean.</param>
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

        // TODO: Handle error
        if (Config.AssemblerConfig is null)
            return null;

        try
        {
            using var stream = File.OpenRead(file.FullPath);
            AssemblerBuildResult result = Config.AssemblerConfig switch
            {
                RasmConfig config => await Assembler.MIPS.Assembler.AssembleAsync<RasmModule, RasmConfig>(stream, file.Name, config, logger),
                ElfConfig config => await Assembler.MIPS.Assembler.AssembleAsync<ElfModule, ElfConfig>(stream, file.Name, config, logger),
                _ => ThrowHelper.ThrowArgumentException<AssemblerBuildResult>(nameof(Config.AssemblerConfig)),
            };

            // Write the object file if the build succeeded
            if (!result.Failed)
            {
                using var outStream = File.Open(file.ObjectFile.FullPath, FileMode.OpenOrCreate);
                result.Module?.Save(outStream);
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

    private bool CleanFile(SourceFile file)
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
