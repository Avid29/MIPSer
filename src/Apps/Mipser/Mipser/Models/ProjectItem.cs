// Avishai Dernis 2025

namespace Mipser.Models;

/// <summary>
/// A base class for an item belonging to a <see cref="Project"/>.
/// </summary>
public abstract class ProjectItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectItem"/> class.
    /// </summary>
    protected ProjectItem(Project project)
    {
        Project = project;
    }

    /// <summary>
    /// Gets the project the item belongs to.
    /// </summary>
    public Project Project { get; }
}
