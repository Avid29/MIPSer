// Avishai Dernis 2026

using System;

namespace Zarem.Registry.Descriptors;

/// <summary>
/// An interface for a class describing an emulator.
/// </summary>
public interface IEmulatorDescriptor : IDescriptor
{
    /// <summary>
    /// Gets the <see cref="Type"/> of the emulator.
    /// </summary>
    Type EmulatorType { get; }
}
