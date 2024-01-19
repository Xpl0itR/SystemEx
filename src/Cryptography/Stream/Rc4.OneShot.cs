// Copyright © 2023-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.Stream;

partial class Rc4
{
    private const int KeyLengthMax = 256;

    public static void XorBlock(ReadOnlySpan<byte> key, Span<byte> block)
    {
        Guard.IsBetweenOrEqualTo(key.Length, 1, KeyLengthMax);

        Span<byte> s = stackalloc byte[KeyLengthMax];
        InitState(key, s);

        for (int i = 0, j = 0, k = 0; k < block.Length; k++)
        {
            block[k] ^= NextByte(s, ref i, ref j);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitState(ReadOnlySpan<byte> key, Span<byte> s)
    {
        for (int i = 0; i < KeyLengthMax; i++)
        {
            s[i] = (byte)i;
        }

        for (int i = 0, j = 0; i < KeyLengthMax; i++)
        {
            j = (j + s[i] + key[i % key.Length]) % KeyLengthMax;

            (s[i], s[j]) = (s[j], s[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte NextByte(Span<byte> s, ref int i, ref int j)
    {
        i = (i + 1)    % KeyLengthMax;
        j = (j + s[i]) % KeyLengthMax;

        (s[i], s[j]) = (s[j], s[i]);

        return s[(s[i] + s[j]) % KeyLengthMax];
    }
}