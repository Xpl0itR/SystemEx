using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SystemEx.IO;

public static class StreamExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReadExactly(this Stream stream, byte* buffer, long length) =>
        ReadExactly(stream, ref *buffer, length);

    public static void ReadExactly(this Stream stream, ref byte buffer, long length)
    {
        while (length > 0)
        {
            int toRead = unchecked((int)Math.Min(length, int.MaxValue));
            int read = stream.Read(
                MemoryMarshal.CreateSpan(ref buffer, toRead));

            if (read == 0)
                ThrowHelpers.ThrowEndOfStreamException();

            length -= read;
            buffer  = ref Unsafe.Add(ref buffer, read);
        }
    }
}