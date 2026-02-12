// Avishai Dernis 2025

using ObjectFiles.Elf;
using ObjectFiles.Elf.Config;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler;
using Zarem.Assembler.Config;
using Zarem.Assembler.Logging;
using Zarem.Assembler.Models;
using Zarem.Models;
using Zarem.Models.Files;

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


            // TODO: Select assembler by architecture
            AssemblerResult result = await MIPSAssembler.AssembleAsync(stream, file.Name,(MIPSAssemblerConfig)Config.AssemblerConfig, logger);

            // Write the object file if the build succeeded
            if (!result.Failed && result.Module is not null)
            {
                // TODO: Select module type by format
                var module = ElfModule.Create(result.Module, new ElfConfig());

                using var outStream = File.Open(file.ObjectFile.FullPath, FileMode.OpenOrCreate);
                module?.Save(outStream);
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
