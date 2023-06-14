// Adam Dernis 2023

using CommunityToolkit.Mvvm.ComponentModel;

namespace Mipser.Bindables;

/// <summary>
/// An file in the content view.
/// </summary>
public class File : ObservableObject
{
    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string? Name { get; }
}
