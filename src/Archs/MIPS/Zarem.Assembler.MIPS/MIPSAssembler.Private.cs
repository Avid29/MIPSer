// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using System.Numerics;
using Zarem.Assembler.Extensions;
using Zarem.Assembler.Logging.Enum;
using Zarem.Assembler.Tokenization.Models;
using Zarem.Models.Addressing;
using Zarem.Models.Modules.Tables.Enums;

namespace Zarem.Assembler;

public partial class MIPSAssembler
{
    /// <remarks>
    /// Bytes should be passed in as big endian.
    /// </remarks>
    private void Append(params byte[] bytes) => _activeSection.Append(bytes);

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

    private void Align(int boundary) => _activeSection.Align(boundary);

    private void SetActiveSection(string sectionName)
    {
        if (!_module.Sections.TryGetValue(sectionName, out var section))
            return;

        _activeSection = section;
    }
    /// <summary>
    /// Defines a label at the current address.
    /// </summary>
    /// <remarks>
    /// At this stage, the label is expected to be passed in with a tailing ':' that will be trimmed.
    /// The method will still work if the semicolon is pre-trimmed.
    /// </remarks>
    /// <param name="label">The name of the symbol.</param>
    private bool DefineLabel(Token label) => DefineSymbol(label, CurrentAddress, SymbolType.Label);

    /// <summary>
    /// Defines a symbol.
    /// </summary>
    /// <param name="label">The name of the symbol.</param>
    /// <param name="address">The value of the symbol.</param>
    /// <param name="type">The symbol type.</param>
    /// <returns>True if successful, false on failure.</returns>
    private bool DefineSymbol(Token label, Address address, SymbolType type)
    {
        // Ensure the symbol has a valid name
        if (!ValidateSymbolName(label, out var name))
            return false;

        // Define the symbol or update by adding flags, address or type.
        // NOTE: The type can only be updated if it is currently unknown
        //       and the address can only be updated if it's undeclared/external.
        if (!_module.TryDefineOrUpdateSymbol(name, type, address))
        {
            _logger?.Log(Severity.Error, LogId.DuplicateSymbolDefinition, label, "SymbolAlreadyDefined", name);
            return false;
        }

        return true;
    }
}
