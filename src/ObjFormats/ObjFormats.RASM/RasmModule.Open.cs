// Avishai Dernis 2025

using System.IO;

namespace ObjFormats.RASM;

public partial class RasmModule
{
    /// <inheritdoc/>
    public static RasmModule? Open(string? name, Stream stream)
    {
        if (!Header.TryRead(stream, out var header))
            return null;

        return new RasmModule(name, header, stream);
    }
}
