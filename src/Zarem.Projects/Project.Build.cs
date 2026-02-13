// Avishai Dernis 2025

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Zarem.Assembler.Logging;
using Zarem.Assembler.Models;
using Zarem.Models;
using Zarem.Models.Files;

namespace Zarem;

public partial class Project
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

        try
        {
            // Assemble the file
            using var stream = File.OpenRead(file.FullPath);
            var result = await Assemble.AssembleFileAsync(file, rebuild, logger);

            // Not dirty and not rebuilding
            if (result is null)
                return null;

            // Write the object file if assembling succeeded
            bool exported = false;
            if (!result.Failed && result.Module is not null)
            {
                exported = await Format.TryExportAsync(result.Module, file.ObjectFile);
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
