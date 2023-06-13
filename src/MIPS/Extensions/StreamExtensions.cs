// Adam Dernis 2023

using System.Numerics;

namespace System.IO;

/// <summary>
/// A class containing extensions for <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Writes a <see cref="IBinaryInteger{TSelf}"/> to a stream.
    /// </summary>
    /// <typeparam name="T">The <see cref="IBinaryInteger{TSelf}"/> type.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The value to write to the stream.</param>
    public static void Write<T>(this Stream stream, T value)
        where T : unmanaged, IBinaryInteger<T>
    {
        var byteCount = value.GetByteCount();
        Span<byte> bytes = stackalloc byte[byteCount];
        value.TryWriteBigEndian(bytes, out _);
        stream.Write(bytes);
    }
}
