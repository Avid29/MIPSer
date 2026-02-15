// Avishai Dernis 2026

using System;
using Zarem.Emulator;
using Zarem.Emulator.Config;
using Zarem.Registry.Attributes;
using Zarem.Registry.Descriptors;

namespace Zarem.MIPS;

/// <summary>
/// An <see cref="IEmulatorDescriptor"/> for the MIPS emulator.
/// </summary>
[ZaremPlugin]
public class MIPSEmulatorDescriptor : IEmulatorDescriptor
{
    /// <inheritdoc/>
    public string Identifier => "MIPS";

    /// <inheritdoc/>
    public Type ConfigType => typeof(MIPSEmulatorConfig);

    /// <inheritdoc/>
    public Type EmulatorType => typeof(MIPSEmulator);
}
