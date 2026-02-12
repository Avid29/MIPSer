// Avishai Dernis 2026

using System;

namespace Zarem.Factory;

/// <summary>
/// A record containing project configuration type info.
/// </summary>
public record ProjectTypeInfo(string TypeName, Type ProjectType, Type ConfigType);
