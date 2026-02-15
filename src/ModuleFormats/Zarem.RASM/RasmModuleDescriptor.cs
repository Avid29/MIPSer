// Avishai Dernis 2026

using ObjFormats.RASM;
using System;
using Zarem.RASM.Config;
using Zarem.Registry.Descriptors;

namespace Zarem.RASM;

/// <summary>
/// An <see cref="IModuleFormatDescriptor"/> for the elf format.
/// </summary>
public class RasmModuleDescriptor : LocalizedDescriptor<RasmModuleDescriptor>, IModuleFormatDescriptor
{
    /// <inheritdoc/>
    public override string Identifier => "rasm";

    /// <inheritdoc/>
    protected override string ResourceNamespace => "Zarem.RASM.Resources";

    /// <inheritdoc/>
    public string? DisplayName => Localizer["ModuleFormatName"];

    /// <inheritdoc/>
    public override Type ConfigType => typeof(RasmConfig);

    /// <inheritdoc/>
    public Type FormatType => typeof(RasmModule);
}
