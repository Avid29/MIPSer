// Avishai Dernis 2026

using System;

namespace Zarem.Serialization.Registry;

/// <summary>
/// A record containing project configuration type info.
/// </summary>
public record ProjectTypeInfo(string TypeName, Type ProjectType, Type ConfigType);
