// Adam Dernis 2024

using System.Xml.Serialization;

namespace MIPS.Models.Instructions.Enums;

/// <summary>
/// An enum for which version(s) a MIPS feature is supported.
/// </summary>
[Flags]
public enum MipsVersion : byte
{
#pragma warning disable CS1591

    [XmlEnum(Name = "MIPS I")]
    MipsI = 1,

    [XmlEnum(Name = "MIPS II")]
    MipsII = 2,

    [XmlEnum(Name = "MIPS III")]
    MipsIII = 3,

    [XmlEnum(Name = "MIPS IV")]
    MipsIV = 4,

    [XmlEnum(Name = "MIPS V")]
    MipsV = 5,

    [XmlEnum(Name = "MIPS VI")]
    MipsVI = 6, 

#pragma warning restore CS1591
}
