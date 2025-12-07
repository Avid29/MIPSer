// Avishai Dernis 2025

using MIPS.Assembler;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Models;
using Mipser.Models;
using Mipser.Models.Files;
using RASM.Modules;
using RASM.Modules.Config;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Mipser;

public partial class Project
{
    /// <summary>
    /// Builds the project.
    /// </summary>
    public async Task<BuildResult?> BuildAsync(bool rebuild = false, Logger? logger = null)
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

    private async Task<AssemblerResult?> AssembleFileAsync(SourceFile file, bool rebuild = true, Logger? logger = null)
    {
        // Skip if not dirty and not rebuilding
        if (!file.IsDirty && !rebuild)
            return null;

        // TODO: Handle error
        if (Config.AssemblerConfig is null)
            return null;

        try
        {
            AssemblerResult result;
            using var stream = File.OpenRead(file.FullPath);
            using (var outStream = File.Open(file.ObjectFile.FullPath, FileMode.OpenOrCreate))
            {
                result = await Assembler.AssembleAsync<RasmModule, RasmConfig>(stream, file.Name, Config.AssemblerConfig, outStream, logger);
            }

            // Delete the object file if the build failed
            if (result.Failed)
            {
                File.Delete(file.ObjectFile.FullPath);
            }

            return result;
        }
        catch
        {
            // TODO: Handle error
            return null;
        }
    }
}
