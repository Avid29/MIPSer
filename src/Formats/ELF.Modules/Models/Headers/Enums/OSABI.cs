// Adam Dernis 2025

namespace ELF.Modules.Models.Headers.Enums;

/// <summary>
/// An enum for the Operating-Systems Application Binary Interface field
/// of the ELF header format.
/// </summary>
public enum OSABI : byte
{
    #pragma warning disable CS1591

    SystemV,
    HP_UX,
    NetBSD,
    Linux,
    GNU_Hard,
    Solaris,
    AIX,
    IRIX,
    FreeBSD,
    Tru64,
    NovellModesto,
    OpenBSD,
    OpenVMS,
    NonStopKernel,
    AROS,
    FenixOS,
    NuxiCloudABI,
    StratusTechnologiesOpenVOS

    #pragma warning restore CS1591
}
