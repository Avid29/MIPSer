// Adam Dernis 2023

namespace MIPS.Models.Modules;

/// <summary>
/// A fully assembled object module.
/// </summary>
public class Module
{
    private Stream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="Module"/> class.
    /// </summary>
    public Module(Stream stream)
    {
        _stream = stream;
    }
}
