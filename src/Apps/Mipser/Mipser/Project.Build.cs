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
    public async Task<AssemblyResult?> BuildAsync()
    {
        // TODO: Assemble dirty files

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
