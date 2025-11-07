// Avishai Dernis 2025

using MIPS.Assembler.Logging;
using System.Collections.Generic;

namespace Mipser.Messages.Build;

/// <summary>
/// A message sent when the build finished, containing status info.
/// </summary>
public class BuildFinishedMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildFinishedMessage"/> class.
    /// </summary>
    public BuildFinishedMessage(bool failed, IReadOnlyList<Log>? log)
    {
        Failed = failed;
        Logs = log;
    }

    /// <summary>
    /// Gets whether or not the build failed.
    /// </summary>
    public bool Failed { get; }

    /// <summary>
    /// Gets the log events from the build.
    /// </summary>
    public IReadOnlyList<Log>? Logs { get; }
}
