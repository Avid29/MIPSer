// Avishai Dernis 2026

using System;

namespace Zarem.Registry.Attributes;

/// <summary>
/// Marks a class as a zarem plugin type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ZaremPluginAttribute : Attribute
{
}
