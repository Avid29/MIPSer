// Avishai Dernis 2025

namespace Mipser.Models.Enums;

/// <summary>
/// An enum indicating the build status.
/// </summary>
public enum BuildStatus
{
    #pragma warning disable CS1591
    
    NotReady,
    Ready,
    Preparing,
    Assembling,
    Linking,
    Completed,
    Failed,
    
    # pragma warning restore CS1591
}
