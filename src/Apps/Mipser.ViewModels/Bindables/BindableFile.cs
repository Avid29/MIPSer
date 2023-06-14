// Adam Dernis 2023

using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace Mipser.Bindables;

/// <summary>
/// An file in the content view.
/// </summary>
public class BindableFile : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    public BindableFile()
    {
        Name = "NewFile";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableFile"/> class.
    /// </summary>
    public BindableFile(FileStream fileStream)
    {
        Name = Path.GetFileName(fileStream.Name);
    }


    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets if the file exists in storage, or just in memory.
    /// </summary>
    public bool IsAnonymous => true;
}
