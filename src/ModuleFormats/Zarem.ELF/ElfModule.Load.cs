// Avishai Dernis 2025

using LibObjectFile.Elf;
using System.IO;
using System.Linq;

namespace Zarem.Elf;

public partial class ElfModule
{
    /// <inheritdoc/>
    public void Load(Stream destination)
    {
        // TODO: Load the module into the stream.
        foreach (var section in _elfFile.Sections.OfType<ElfStreamSection>())
        {
            if (!section.Flags.HasFlag(ElfSectionFlags.Alloc))
                continue;

            var vAddr = (long)section.VirtualAddress;
            if (destination.Length < vAddr)
            {
                destination.SetLength(vAddr);
            }

            destination.Position = vAddr;
            section.Stream.Position = 0;
            section.Stream.CopyTo(destination);
        }
    }
}
