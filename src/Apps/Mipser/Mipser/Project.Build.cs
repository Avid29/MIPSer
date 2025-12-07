// Avishai Dernis 2025

using MIPS.Assembler;
using MIPS.Assembler.Models;
using Mipser.Models.Files;
using RASM.Modules;
using RASM.Modules.Config;
using System.IO;
using System.Threading.Tasks;

namespace Mipser;

public partial class Project
{
    /// <summary>
    /// Builds the project.
    /// </summary>
    /// <param name="rebuild">If false, skip files which are not dirty.</param>
    public async Task<AssemblyResult?> BuildAsync(bool rebuild = false)
    {
        AssemblyResult? result = null;
        foreach(var file in SourceFiles)
        {
            // TODO: Mix AssemblyResult

            result = await AssembleFileAsync(file);

            if (result?.Failed is true)
                return result;
        }

        // TODO: Link

        return null;
    }

    /// <summary>
    /// Assembles a file.
    /// </summary>
    public async Task<AssemblyResult?> AssembleFileAsync(SourceFile file)
    {
        if (Config.AssemblerConfig is null)
            return null;

        try
        {
            using var stream = File.OpenRead(file.FullPath);
            using var outStream = File.Open(file.ObjectFile.FullPath, FileMode.OpenOrCreate);

            return await Assembler.AssembleAsync<RasmModule, RasmConfig>(stream, file.Name, Config.AssemblerConfig, outStream);
        }
        catch
        {
            // TODO: Handle error
            return null;
        }
    }
}
