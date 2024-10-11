// Adam Dernis 2024

using System.Buffers;
using System.Numerics;

namespace MIPS.Extensions.System.IO;

/// <summary>
/// A class containing extensions for <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Reads a <see cref="IBinaryInteger{TSelf}"/> from a stream.
    /// </summary>
    /// <remarks>
    /// MIPS is big endian, so this method uses big endian.
    /// </remarks>
    /// <typeparam name="T">The <see cref="IBinaryInteger{TSelf}"/> type.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The next <typeparamref name="T"/> from the stream.</param>
    /// <returns><see cref="true"/> if value was successfully read. <see cref="false"/> otherwise.</returns>
    public static bool TryRead<T>(this Stream stream, out T value)
        where T : unmanaged, IBinaryInteger<T>
    {
        // Initialize results
        bool success = false;
        value = default;

        // Create temporary array
        byte[]? pooledArray = null;
        var byteCount = value.GetByteCount();
        var bytes = byteCount <= 8 ?
            stackalloc byte[byteCount] :
            (pooledArray = ArrayPool<byte>.Shared.Rent(byteCount));

        // Read bytes and parse
        int realCount = stream.Read(bytes);
        if (realCount == byteCount)
        {
            success = T.TryReadBigEndian(bytes, false, out value);
        }

        // Free temporary array
        if (pooledArray is not null)
        {
            ArrayPool<byte>.Shared.Return(pooledArray);
        }

        return success;
    }

    /// <summary>
    /// Writes a <see cref="IBinaryInteger{TSelf}"/> to a stream.
    /// </summary>
    /// <remarks>
    /// MIPS is big endian, so this method uses big endian.
    /// </remarks>
    /// <typeparam name="T">The <see cref="IBinaryInteger{TSelf}"/> type.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The value to write to the stream.</param>
    /// <returns><see cref="true"/> if value was successfully written. <see cref="false"/> otherwise.</returns>
    public static bool TryWrite<T>(this Stream stream, T value)
        where T : unmanaged, IBinaryInteger<T>
    {
        // Initialize results
        bool success = false;
        value = default;

        // Create temporary array
        byte[]? pooledArray = null;
        var byteCount = value.GetByteCount();
        var bytes = byteCount <= 8 ?
            stackalloc byte[byteCount] :
            (pooledArray = ArrayPool<byte>.Shared.Rent(byteCount));

        // Write bytes
        success = value.TryWriteBigEndian(bytes, out var bytesWritten);
        if (success && bytesWritten == byteCount)
        {
            stream.Write(bytes);
        }

        // Free temporary array
        if (pooledArray is not null)
        {
            ArrayPool<byte>.Shared.Return(pooledArray);
        }

        return success;
    }
}
