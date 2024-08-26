// Copyright © 2024 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

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