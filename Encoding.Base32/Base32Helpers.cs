// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

namespace SystemEx.Encoding.Base32;

public static partial class Base32
{
    public static byte[] ToBytes(ReadOnlySpan<char> source)
    {
        source = source.TrimEnd('=');

        if (source.IsEmpty)
        {
            return Array.Empty<byte>();
        }

        byte[] destination = new byte[NumBytes(source)];
        ToBytes(source, destination);

        return destination;
    }

    public static char[] ToChars(ReadOnlySpan<byte> source)
    {
        if (source.IsEmpty)
        {
            return Array.Empty<char>();
        }

        char[] destination = new char[NumChars(source)];
        ToChars(source, destination);

        return destination;
    }

    public static string ToString(byte[] source) =>
        source.Length == 0
            ? string.Empty
            : string.Create(NumChars(source),
                            source, // Sadly string.Create does not accept spans
                            (destination, src) =>
                                ToChars(src, destination));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NumBytes(ReadOnlySpan<char> source) =>
        source.Length * 5 / 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NumBytes(char[] source) =>
        source.Length * 5 / 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NumBytes(string source) =>
        source.Length * 5 / 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NumChars(ReadOnlySpan<byte> source) =>
        (int)Math.Ceiling(source.Length / 5d) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NumChars(byte[] source) =>
        (int)Math.Ceiling(source.Length / 5d) * 8;
}