// Adam Dernis 2023

using System.Numerics;

namespace MIPS.Extensions.System.IO;

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
    /// <returns>The next <typeparamref name="T"/> from the stream.</returns>
    public static T Read<T>(this Stream stream)
        where T : unmanaged, IBinaryInteger<T>
    {
        T value = default;
        var byteCount = value.GetByteCount();
        var bytes = new byte[byteCount];
        stream.ReadExactly(bytes, 0, byteCount);
        T.TryReadBigEndian(bytes, false, out value);
        return value;
    }

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
