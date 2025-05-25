// Copyright © 2022-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using SystemEx.Memory;

namespace SystemEx.Encoding;

partial class Base32
{
    public static void GetBytes(ReadOnlySpan<char> source, Span<byte> destination)
    {
        Guard.HasSizeGreaterThan(source,      0);
        Guard.HasSizeGreaterThan(destination, CountBytes(source.Length));

        GetBytesUnchecked(source, destination);
    }

    public static void GetChars(ReadOnlySpan<byte> source, Span<char> destination)
    {
        Guard.HasSizeGreaterThan(source,      0);
        Guard.HasSizeGreaterThan(destination, CountChars(source.Length));

        GetCharsUnchecked(source, destination);
    }

    public static byte[] GetBytes(ReadOnlySpan<char> source)
    {
        source = source.TrimEnd('=');

        if (source.IsEmpty)
        {
            return [];
        }

        int count = CountBytes(source.Length);
        byte[] destination =
#if NET5_0_OR_GREATER
            GC.AllocateUninitializedArray<byte>(count);
#else
            new byte[count];
#endif

        GetBytesUnchecked(source, destination);
        
        return destination;
    }

    public static char[] GetChars(ReadOnlySpan<byte> source)
    {
        if (source.IsEmpty)
        {
            return [];
        }

        int count = CountChars(source.Length);
        char[] destination =
#if NET5_0_OR_GREATER
            GC.AllocateUninitializedArray<char>(count);
#else
            new char[count];
#endif

        GetCharsUnchecked(source, destination);

        return destination;
    }

    public static string GetString(ReadOnlySpan<byte> source)
    {
        if (source.IsEmpty)
        {
            return string.Empty;
        }

        int count = CountChars(source.Length);

#if NET9_0_OR_GREATER
        return string.Create(count, source, static (destination, source) =>
            GetCharsUnchecked(source, destination));
#else
        string destination = new('\0', count);
        GetCharsUnchecked(source, destination.AsWriteableSpan());

        return destination;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountBytes(int charCount) =>
        charCount * 5 / 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountChars(int byteCount) =>
        (int)Math.Ceiling(byteCount / 5d) * 8;
}