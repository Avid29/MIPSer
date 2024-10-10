// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Models.Addressing.Enums;
using System.Numerics;

namespace MIPS.Assembler;

public partial class Assembler
{
    /// <summary>
    /// Bytes should be passed in as big endian.
    /// </summary>
    private void Append(params byte[] bytes) => _module.Append(_activeSection, bytes);

    private void Append<T>(params T[] values)
        where T : IBinaryInteger<T>
    {
        foreach (var value in values)
        {
            var bytes = new byte[value.GetByteCount()];
            value.WriteBigEndian(bytes);
            Append(bytes);
        }
    }

    private void Append(int byteCount)
    {
        Guard.IsGreaterThanOrEqualTo(byteCount, 0);
        Append(new byte[byteCount]);
    }

    private void Align(int boundary) => _module.Align(_activeSection, boundary);

    private void SetActiveSection(Section section) => _activeSection = section;
}
