// Avishai Dernis 2025

using Mipser.Models.Enums;

namespace Mipser.Messages.Build;

/// <summary>
/// A message sent when the build status changes.
/// </summary>
public class BuildStatusMessage
{
    /// <summary>
    /// Initialzes a new instance of the <see cref="BuildStatusMessage"/> class.
    /// </summary>
    public BuildStatusMessage(BuildStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Gets the new build status.
    /// </summary>
    public BuildStatus Status { get; }
}
