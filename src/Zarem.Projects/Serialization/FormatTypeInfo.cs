// Avishai Dernis 2026

using System;

namespace Zarem.Serialization;

/// <summary>
/// A record containing project configuration type info.
/// </summary>
public record FormatTypeInfo(string TypeName, Type FormatType, Type ConfigType);
