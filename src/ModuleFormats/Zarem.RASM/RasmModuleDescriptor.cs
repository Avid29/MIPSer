// Avishai Dernis 2026

using ObjFormats.RASM;
using System;
using Zarem.RASM.Config;
using Zarem.Registry.Descriptors;

namespace Zarem.RASM;

/// <summary>
/// An <see cref="IModuleFormatDescriptor"/> for the elf format.
/// </summary>
public class RasmModuleDescriptor : IModuleFormatDescriptor
{
    /// <inheritdoc/>
    public string Identifier => "rasm";

    /// <inheritdoc/>
    public Type ConfigType => typeof(RasmConfig);

    /// <inheritdoc/>
    public Type FormatType => typeof(RasmModule);
}
