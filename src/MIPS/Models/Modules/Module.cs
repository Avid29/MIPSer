// Adam Dernis 2024

namespace MIPS.Models.Modules;

/// <summary>
/// A fully assembled object module.
/// </summary>
public class Module
{
    private Header _header;
    private Stream _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="Module"/> class.
    /// </summary>
    public Module(Header header, Stream source)
    {
        _header = header;
        _source = source;
    }

    /// <summary>
    /// Loads a module from a stream.
    /// </summary>
    /// <returns>The module contained in the stream.</returns>
    public static Module? Load(Stream stream)
    {
        if(!Header.TryReadHeader(stream, out var header))
            return null;

        return new Module(header, stream);
    }
}
