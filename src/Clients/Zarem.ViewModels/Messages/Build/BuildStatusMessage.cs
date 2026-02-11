// Avishai Dernis 2025

using Zarem.Models.Enums;

namespace Zarem.Messages.Build;

/// <summary>
/// A message sent when the build status changes.
/// </summary>
public class BuildStatusMessage
{
    /// <summary>
    /// Initialzes a new instance of the <see cref="BuildStatusMessage"/> class.
    /// </summary>
    public BuildStatusMessage(BuildStatus status, string? message = null)
    {
        Status = status;
        Message = message;
    }

    /// <summary>
    /// Gets the new build status.
    /// </summary>
    public BuildStatus Status { get; }

    /// <summary>
    /// Gets the build status message.
    /// </summary>
    public string? Message { get; }
}
