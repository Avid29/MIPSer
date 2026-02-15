// Avishai Dernis 2026

using System;

namespace Zarem.Registry.Descriptors;

/// <summary>
/// An interface for a class describing a supported architecture.
/// </summary>
public interface IModuleFormatDescriptor : IDescriptor
{
    /// <summary>
    /// Gets the <see cref="Type"/> of the format.
    /// </summary>
    Type FormatType { get; }
}
