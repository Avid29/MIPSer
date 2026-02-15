// Avishai Dernis 2026

using System;
using Zarem.Elf.Config;
using Zarem.Registry.Descriptors;

namespace Zarem.Elf;

/// <summary>
/// An <see cref="IModuleFormatDescriptor"/> for the elf format.
/// </summary>
public class ElfModuleDescriptor : IModuleFormatDescriptor
{
    /// <inheritdoc/>
    public string Identifier => "ELF";

    /// <inheritdoc/>
    public Type ConfigType => typeof(ElfConfig);

    /// <inheritdoc/>
    public Type FormatType => typeof(ElfModule);
}
