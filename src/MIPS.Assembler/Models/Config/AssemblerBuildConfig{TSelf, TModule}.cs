// Avishai Dernis 2025

using MIPS.Assembler.Models.Modules;
using MIPS.Assembler.Models.Modules.Interfaces;
using MIPS.Models.Instructions.Enums;
using System.IO;

namespace MIPS.Assembler.Models.Config;

/// <summary>
/// A class for a build ready <see cref="AssemblerConfig"/>.
/// </summary>
public class AssemblerBuildConfig<TSelf, TModule> : AssemblerBuildConfig
    where TSelf : AssemblerBuildConfig<TSelf, TModule>
    where TModule : IBuildModule<TModule>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerBuildConfig{TSelf, TModule}"/> class.
    /// </summary>
    public AssemblerBuildConfig(MipsVersion version = MipsVersion.MipsII) : base(version)
    {
    }

    /// <inheritdoc/>
    public TModule? CreateModule(Module module, TSelf config, Stream? stream = null)
        => TModule.Create(module, config, stream);

    /// <inheritdoc/>
    public override IBuildModule? CreateModule(Module module, AssemblerConfig config, Stream? stream = null)
    {
        if (config is not TSelf selfConfig)
            return null;

        return CreateModule(module, selfConfig, stream);
    }
}
