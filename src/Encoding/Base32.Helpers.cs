// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using SystemEx.HighPerformance;

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

    public static string GetString(ReadOnlySpan<byte> source)
    {
        if (source.Length == 0)
        {
            return string.Empty;
        }

        string destination = new('\0', CountChars(source.Length));
        GetChars(source, destination.AsWriteableSpan());

        return destination;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountBytes(int charCount) =>
        charCount * 5 / 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountChars(int byteCount) =>
        (int)Math.Ceiling(byteCount / 5d) * 8;
}