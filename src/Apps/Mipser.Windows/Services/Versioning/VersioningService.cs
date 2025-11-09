// Avishai Dernis 2025

using Mipser.Services.Versioning;
using Mipser.Services.Versioning.Models;
using Windows.ApplicationModel;

namespace Mipser.Windows.Services.Versioning;

/// <summary>
/// An implementation of the <see cref="IVersioningService"/>.
/// </summary>
public class VersioningService : IVersioningService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VersioningService"/> class.
    /// </summary>
    public VersioningService()
    {
        var packageVersion = Package.Current.Id.Version;
        AppVersion = new AppVersion
        {
            MajorVersion = packageVersion.Major,
            MinorVersion = packageVersion.Minor,
            Revision = packageVersion.Revision,
        };

        GitVersionInfo = new GitVersionInfo
        {
            Branch = ThisAssembly.Git.Branch,
            Commit = ThisAssembly.Git.Commit,
        };
    }

    /// <inheritdoc/>
    public AppVersion AppVersion { get; }
    
    /// <inheritdoc/>
    public GitVersionInfo GitVersionInfo { get; }
}
