// Avishai Dernis 2026

using System;
using Zarem.Attributes;

namespace Zarem.Serialization.Registry;

/// <summary>
/// A record containing project configuration type info.
/// </summary>
public record ProjectTypeInfo(string TypeName, Type ConfigType, ProjectTypeAttribute ProjectType);
