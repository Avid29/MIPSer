// Avishai Dernis 2026

using System;
using System.Diagnostics;

namespace Zarem.Assembler.Logging;

/// <summary>
/// A code for a log type.
/// </summary>
[DebuggerDisplay(nameof(ToString))]
public readonly struct LogCode(string provider, uint code)
{
    /// <summary>
    /// Gets identifying code for the component that made the log.
    /// </summary>
    public string Provider { get; } = provider;

    /// <summary>
    /// Gets the provider's code for log type.
    /// </summary>
    public uint Id { get; } = code;

    /// <inheritdoc/>
    public static bool operator ==(LogCode left, LogCode right)
        => left.Id == right.Id && left.Provider == right.Provider;

    /// <inheritdoc/>
    public static bool operator !=(LogCode left, LogCode right)
        => left.Id != right.Id || left.Provider != right.Provider;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is LogCode code)
            return this == code;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Id, Provider);

    /// <inheritdoc/>
    public readonly override string ToString() => $"{Provider}_{Id:0000}";
}
