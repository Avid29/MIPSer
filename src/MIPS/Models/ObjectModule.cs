// Adam Dernis 2023

namespace MIPS.Models;

/// <summary>
/// A fully assembled object module.
/// </summary>
public class ObjectModule
{
    private byte[] _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectModule"/> class.
    /// </summary>
    public ObjectModule()
    {
        _data = Array.Empty<byte>();
    }
}
