// Avishai Dernis 2025

using MIPS.Assembler.Logging;

namespace Mipser.Messages.Build;

/// <summary>
/// A message sent when the build started, containing the logger.
/// </summary>
public class BuildStartedMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    public BuildStartedMessage(Logger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Gets the logger being used for assembly.
    /// </summary>
    public Logger Logger { get; }
}
