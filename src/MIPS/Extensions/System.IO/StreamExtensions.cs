// Adam Dernis 2024

using System.Buffers;
using System.Numerics;

namespace System.IO;

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
        // TODO: This doesn't feel like it should be neccesary.
        bool signed = (-T.MultiplicativeIdentity) < T.Zero;

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
            success = T.TryReadBigEndian(bytes, !signed, out value);
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
    
    /// <summary>
    /// Copies a set number of bytes from <paramref name="source"/> to <paramref name="destination"/>.
    /// </summary>
    /// <param name="destination">The destination stream.</param>
    /// <param name="source">The source stream.</param>
    /// <param name="bytes">The number of bytes to copy.</param>
    public static void CopyFrom(this Stream destination, Stream source, int bytes)
    {
        byte[] buffer = new byte[32768];
        int read;
        while (bytes > 0 &&  (read = source.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
        {
            destination.Write(buffer, 0, read);
            bytes -= read;
        }
    }
}
