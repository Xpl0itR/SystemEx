// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Encoding;

/// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4648#section-6" /></remarks>
public static partial class Base32
{
    public static void GetBytesUnchecked(ReadOnlySpan<char> source, Span<byte> destination)
    {
        int current   = 0,
            count     = 0,
            remaining = 0;

        foreach (char chr in source)
        {
            current   |= CharToInt(chr);
            remaining += 5;

            if (remaining >= 8)
            {
                destination[count++] = (byte)(current >> (remaining - 8));
                remaining           -= 8;
            }

            current <<= 5;
        }
    }

    public static void GetCharsUnchecked(ReadOnlySpan<byte> source, Span<char> destination)
    {
        int current   = source[0],
            next      = 1,
            count     = 0,
            remaining = 8;

        while (remaining > 0 || next < source.Length)
        {
            if (remaining < 5)
            {
                if (next < source.Length)
                {
                    current   <<= 8;
                    current   |=  source[next++] & 255;
                    remaining +=  8;
                }
                else
                {
                    int pad = 5 - remaining;
                    current   <<= pad;
                    remaining +=  pad;
                }
            }

            destination[count++] =  IntToChar(current >> (remaining - 5) & 31);
            remaining            -= 5;
        }

        while (count != destination.Length)
        {
            destination[count++] = '=';
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char IntToChar(int i) =>
        (char)(i < 26
                ? i + 'A'
                : i + 24);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CharToInt(char chr)
    {
        if ((uint)(chr - 'A') <= 'Z' - 'A')
        {
            return chr - 'A';
        }

        if ((uint)(chr - '2') <= '7' - '2')
        {
            return chr - 24;
        }

#pragma warning disable HAA0601 // JUSTIFICATION: Boxing of char only occurs on exception
        return ThrowHelper.ThrowArgumentOutOfRangeException<int>(
            nameof(chr), chr, "Source contains character which does not exist in Base32.");
#pragma warning restore HAA0601
    }
}