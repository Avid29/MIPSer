// Avishai Dernis 2026

using System;
using Zarem.Registry.Attributes;
using Zarem.Registry.Descriptors;

namespace Zarem.MIPS;

/// <summary>
/// An <see cref="IArchitectureDescriptor"/> for the MIPS architecture.
/// </summary>
[ZaremPlugin]
public class MIPSArchitectureDescriptor : IArchitectureDescriptor
{
    /// <inheritdoc/>
    public string Identifier => "MIPS";

    /// <inheritdoc/>
    public Type ConfigType => typeof(MIPSArchitectureConfig);

    /// <inheritdoc/>
    public IAssemblerDescriptor Assembler => new MIPSAssemblerDescriptor();

    /// <inheritdoc/>
    public IEmulatorDescriptor Emulator => new MIPSEmulatorDescriptor();
}
