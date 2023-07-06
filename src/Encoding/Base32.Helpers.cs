// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

namespace SystemEx.Encoding;

partial class Base32
{
    public static byte[] GetBytes(ReadOnlySpan<char> source)
    {
        source = source.TrimEnd('=');

        if (source.IsEmpty)
        {
            return Array.Empty<byte>();
        }

        byte[] destination = GC.AllocateUninitializedArray<byte>(
            CountBytes(source.Length));

        GetBytes(source, destination);
        
        return destination;
    }

    public static char[] GetChars(ReadOnlySpan<byte> source)
    {
        if (source.IsEmpty)
        {
            return Array.Empty<char>();
        }

        char[] destination = GC.AllocateUninitializedArray<char>(
            CountChars(source.Length));

        GetChars(source, destination);

        return destination;
    }

    public static unsafe string GetString(ReadOnlySpan<byte> source)
    {
        if (source.Length == 0)
        {
            return string.Empty;
        }

        fixed (byte* sourcePtr = source)
        {
            return string.Create(
                CountChars(source.Length),
                (Ptr: (IntPtr)sourcePtr, source.Length),
                (destination, state) =>
                {
                    ReadOnlySpan<byte> src = new((byte*)state.Ptr, state.Length);
                    GetChars(src, destination);
                });
        }
    }

    public static string GetString(byte[] source)
    {
        if (source.Length == 0)
        {
            return string.Empty;
        }

        return string.Create(
            CountChars(source.Length),
            source,
            (destination, src) => GetChars(src, destination));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountBytes(int charCount) =>
        charCount * 5 / 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountChars(int byteCount) =>
        (int)Math.Ceiling(byteCount / 5d) * 8;
}