// Avishai Dernis 2025

using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance.Helpers;
using MIPS.Emulator.Helpers;
using System;
using System.IO;
using System.Numerics;

namespace MIPS.Emulator.Models.System.Memory;

/// <summary>
/// Represents the RAM (Random Access Memory) in a MIPS emulator.
/// </summary>
public class SystemMemory
{
    private PagedMemoryStream _memoryStream;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMemory"/> class.
    /// </summary>
    public SystemMemory()
    {
        _memoryStream = new PagedMemoryStream(4096);

        // Set the length to the maximum addressable memory (4GB for 32-bit address space).
        // The PagedMemoryStream will handle the actual allocation of memory in pages as needed, so we don't need to allocate all 4GB upfront
        _memoryStream.SetLength(uint.MaxValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public uint this[uint address]
    {
        get => Read<uint>(address);
        set => Write(address, value);
    }

    /// <summary>
    /// Reads a value of type <typeparamref name="T"/> from the specified address.
    /// </summary>
    /// <typeparam name="T">The type of the value to read.</typeparam>
    /// <param name="address">The address to read from.</param>
    /// <returns>The value at the address as a <typeparamref name="T"/>.</returns>
    public T Read<T>(uint address)
        where T : unmanaged, IBinaryInteger<T>
    {
        _memoryStream.Position = address;
        if (_memoryStream.TryRead(out T value))
            return value;

        return default;
    }

    /// <summary>
    /// Writes a value of type <typeparamref name="T"/> to the specified address.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="address">The address to write to.</param>
    /// <param name="value">The value to write.</param>
    public void Write<T>(uint address, T value)
        where T : unmanaged, IBinaryInteger<T>
    {
        _memoryStream.Position = address;
        _memoryStream.TryWrite(value);
    }

    /// <summary>
    /// Reads a byte array from the specified address into the provided buffer.
    /// </summary>
    public void Read(uint address, byte[] buffer, bool endianCheck = false)
    {
        // NOTE: Handle the case where the buffer is larger than the remaining memory, or if the address is out of bounds

        _memoryStream.Position = address;
        _memoryStream.ReadExactly(buffer);

        // Handle endianess
        if(endianCheck && BitConverter.IsLittleEndian)
            buffer.Reverse();
    }

    /// <summary>
    /// Writes a byte array to the specified address.
    /// </summary>
    public void Write(uint address, byte[] bytes, bool endianCheck = false)
    {
        // Handle endianess
        if (endianCheck && BitConverter.IsLittleEndian)
            bytes.Reverse();

        _memoryStream.Position = address;
        _memoryStream.Write(bytes);
    }

    /// <summary>
    /// Gets the RAM memory as a <see cref="Stream"/>.
    /// </summary>
    public Stream AsStream() => _memoryStream;
}
